using FileHosting.Database;
using FileHosting.Database.Models;
using FileHosting.Domain.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using FileHosting.Services.Extensions;
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

        public File GetFileToDownload(int fileId, User user)
        {
            var file = GetFileById(fileId);

            if (file == null) return null;

            _context.DownloadRepository.Add(new Download
            {
                File = file,
                User = user
            });
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
                Size = decimal.Round((decimal)fileSize / 1024, 2),
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

        public void ChangeFile(File file, string fileTags, string fileDescription)
        {
            _context.FileRepository.Attach(file);

            var fileTagsList = fileTags.ToTagsList();
            if (fileTagsList != null)
            {
            }

            file.Tags = fileTags;
            file.Description = fileDescription;

            _context.Commit();
        }

        public List<CommentModel> GetCommentsForFile(int fileId)
        {
            var comments = _context.CommentRepository.Find(c => c.File.Id == fileId && c.IsActive)
                .OrderBy(c => c.Id)
                .ToList();

            if (!comments.Any())
                return null;

            var commentModelList = new List<CommentModel>(comments.Count);
            commentModelList.AddRange(comments.Select((c, i) => new CommentModel
            {
                CommentId = c.Id,
                Number = (i + 1),
                Author = c.Author == null ? "Guest" : c.Author.Name,
                PublishDate = c.PublishDate,
                Text = c.Text
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

        public void DeleteCommentFromFile(int commentId)
        {
            var comment = _context.CommentRepository.GetById(commentId);

            _context.CommentRepository.Delete(comment);

            _context.Commit();
        }
    }
}