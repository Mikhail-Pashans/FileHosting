using FileHosting.Database;
using FileHosting.Database.Models;
using FileHosting.Domain.Models;
using FileHosting.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using File = FileHosting.Database.Models.File;

namespace FileHosting.Services
{
    public class FileService
    {
        private readonly IUnitOfWork _context;

        public FileService()
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        public List<FileModel> GetFilesForSection(int sectionId)
        {
            return _context.FileRepository.Find(f => f.Section.Id == sectionId)
                .Select(f => new FileModel
                {
                    Id = f.Id,
                    Section = f.Section,
                    Name = f.Name,
                    Size = f.Size,
                    UploadDate = f.UploadDate
                })
                .OrderByDescending(f => f.Id)
                .ToList();
        }

        public List<FileModel> GetFilesForUser(int userId)
        {
            return _context.FileRepository.Find(f => f.Owner.Id == userId)
                .Select(f => new FileModel
                {
                    Id = f.Id,
                    Section = f.Section,
                    Name = f.Name,
                    Size = f.Size,
                    UploadDate = f.UploadDate
                })
                .OrderByDescending(f => f.Id)
                .ToList();
        }

        public File GetFileById(int fileId)
        {
            return _context.FileRepository.FirstOrDefault(f => f.Id == fileId);
        }

        public File GetFileToDownload(int fileId)
        {
            var file = GetFileById(fileId);

            return file;
        }

        public bool WriteDownload(File file, User user)
        {
            _context.DownloadRepository.Add(new Download
            {
                Date = DateTime.UtcNow,
                File = file,
                User = user
            });

            try
            {
                _context.Commit();
            }
            catch (DataException)
            {
                return false;
            }

            return true;
        }

        public FileModel GetModelForFile(File file, bool isAuthenticated)
        {
            return isAuthenticated
                ? new FileModel
                {
                    Id = file.Id,
                    Name = file.Name,
                    Section = file.Section,
                    Tags = file.Tags.Any() ? file.Tags.ToTagsString() : "",
                    Description = string.IsNullOrWhiteSpace(file.Description) ? "" : file.Description,
                    UploadDate = file.UploadDate,
                    Size = file.Size,
                    Path = file.Path,
                    Downloads = file.Downloads.ToList(),
                    IsAllowedAnonymousBrowsing = file.IsAllowedAnonymousBrowsing,
                    IsAllowedAnonymousAction = file.IsAllowedAnonymousAction
                }
                : new FileModel
                {
                    Id = file.Id,
                    Name = file.Name,
                    Section = file.Section,
                    Tags = file.Tags.Any() ? file.Tags.ToTagsString() : "N/A",
                    Description = string.IsNullOrWhiteSpace(file.Description) ? "N/A" : file.Description,
                    UploadDate = file.UploadDate,
                    Size = file.Size,
                    Path = file.Path,
                    Owner = file.Owner,
                    IsAllowedAnonymousBrowsing = file.IsAllowedAnonymousBrowsing,
                    IsAllowedAnonymousAction = file.IsAllowedAnonymousAction
                };
        }

        public void AddFile(string fileName, string fullName, long fileSize, string filePath, string ipAddress, Section fileSection, User owner)
        {
            var files = _context.FileRepository.Find(f => f.FullName == fullName).ToArray();
            if (files.Any())
            {
                foreach (var file in files)
                {
                    DeleteFile(file, ipAddress);
                }
            }

            _context.FileRepository.Add(new File
            {
                Name = fileName,
                FullName = fullName,
                Section = fileSection,
                UploadDate = DateTime.UtcNow,
                Size = fileSize,
                Path = filePath,
                Owner = owner,
                IsAllowedAnonymousBrowsing = true,
                IsAllowedAnonymousAction = true,
                AllowedUsers = new List<User>(),
                Comments = new List<Comment>(),
                Downloads = new List<Download>(),
                Tags = new List<Tag>()
            });

            _context.Commit();
        }

        public void DeleteFile(File file, string ipAddress, bool multiple = true)
        {
            _context.FileRepository.Delete(file);

            if (!multiple)
                _context.Commit();

            var filePath = Path.Combine(ipAddress, file.Path);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        public void ChangeFile(File file, string fileTagsString, string fileDescription)
        {
            _context.FileRepository.Attach(file);

            var newFileTags = fileTagsString.ToTagsArray();
            
            if (newFileTags != null)
            {
                var fileTags = new List<Tag>(newFileTags.Length);
                var existingTags = _context.TagRepository.GetAll().Select(t => t.Name).ToArray();

                foreach (var newFileTag in newFileTags)
                {
                    if (existingTags.Contains(newFileTag))
                    {
                        var tag = newFileTag;
                        fileTags.Add(_context.TagRepository.First(t => t.Name == tag));
                    }
                    else
                    {
                        var newTag = new Tag { Name = newFileTag, Files = new List<File>() };
                        
                        _context.TagRepository.Add(newTag);
                        
                        fileTags.Add(newTag);
                    }
                }

                file.Tags = fileTags;
            }
            else
            {
                file.Tags.Clear();
            }

            file.Description = fileDescription;

            _context.Commit();
        }

        public List<CommentModel> GetCommentsForFile(int fileId, bool isForFileOwner)
        {
            var comments = isForFileOwner
                ? _context.CommentRepository.Find(c => c.File.Id == fileId).ToArray()
                : _context.CommentRepository.Find(c => c.File.Id == fileId && c.IsActive).ToArray();
            
            if (!comments.Any())
                return null;

            var commentModelList = new List<CommentModel>(comments.Length);
            commentModelList.AddRange(comments.OrderBy(c => c.Id).Select((c, i) => new CommentModel
            {
                Id = c.Id,
                Number = (i + 1),
                Author = c.Author == null ? "Guest" : c.Author.Name,
                PublishDate = c.PublishDate,
                Text = c.Text,
                IsActive = c.IsActive
            }));

            return commentModelList.OrderByDescending(c => c.Number).ToList();
        }

        public void AddCommentToFile(string commentText, File file, User user)
        {
            var comment = new Comment
            {
                Text = commentText,
                PublishDate = DateTime.UtcNow,
                IsActive = true,
                File = file,
                Author = user
            };
            _context.CommentRepository.Add(comment);

            _context.Commit();
        }

        public bool DeleteCommentFromFile(int commentId)
        {
            var comment = _context.CommentRepository.GetById(commentId);
            if (comment == null)
                return false;

            _context.CommentRepository.Delete(comment);

            try
            {
                _context.Commit();
            }
            catch (DataException)
            {
                return false;
            }

            return true;
        }

        public bool ChangeCommentState(int commentId, bool isActive)
        {
            var comment = _context.CommentRepository.GetById(commentId);
            if (comment == null)
                return false;

            _context.CommentRepository.Attach(comment);

            comment.IsActive = isActive;

            try
            {
                _context.Commit();
            }
            catch (DataException)
            {
                return false;
            }

            return true;
        }
    }
}