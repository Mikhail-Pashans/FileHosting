using System.Configuration;
using System.IO;
using FileHosting.Database;
using FileHosting.Database.Models;
using FileHosting.Domain.Models;
using System;
using System.Collections.Generic;
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

        public FileModel GetModelForFile(File file, bool isAuthenticated)
        {
            return isAuthenticated
                ? new FileModel
                {
                    Id = file.Id,
                    Name = file.Name,
                    Section = file.Section,
                    Tags = file.Tags.ToList(),
                    Description = file.Description,
                    UploadDate = file.UploadDate,
                    Size = file.Size,
                    Path = file.Path,
                    IsAllowedAnonymousBrowsing = file.IsAllowedAnonymousBrowsing,
                    IsAllowedAnonymousComments = file.IsAllowedAnonymousComments
                }
                : new FileModel
                {
                    Id = file.Id,
                    Name = file.Name,
                    Section = file.Section,
                    Tags = file.Tags.Any() ? file.Tags.ToList() : null,
                    Description = string.IsNullOrWhiteSpace(file.Description) ? "N/A" : file.Description,
                    UploadDate = file.UploadDate,
                    Size = file.Size,
                    Path = file.Path,
                    Owner = file.Owner,
                    IsAllowedAnonymousBrowsing = file.IsAllowedAnonymousBrowsing,
                    IsAllowedAnonymousComments = file.IsAllowedAnonymousComments
                };
        }

        public void AddFile(string fileName, string fullName, long fileSize, string filePath, string ipAddress, Section fileSection, User owner)
        {
            var files = _context.FileRepository.Find(f => f.FullName == fullName).ToArray();
            if (files.Any())
            {
                foreach (var file in files)
                {
                    DeleteFile(file, ipAddress, true);
                }
            }

            var newFile = new File
            {
                Name = fileName,
                FullName = fullName,
                Section = fileSection,
                UploadDate = DateTime.UtcNow,
                Size = decimal.Round((decimal)fileSize / 1024, 2),
                Path = filePath,
                Owner = owner,
                IsAllowedAnonymousBrowsing = true,
                IsAllowedAnonymousComments = true,
                AllowedUsers = new List<User>(),
                Comments = new List<Comment>(),
                Downloads = new List<Download>(),
                Tags = new List<Tag>()
            };
            _context.FileRepository.Add(newFile);

            _context.Commit();
        }        

        public void DeleteFile(File file, string ipAddress, bool multiple)
        {
            _context.FileRepository.Delete(file);
            
            if(!multiple)
                _context.Commit();

            var filePath = Path.Combine(ipAddress, file.Path);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        //public void ChangeFile(File file, string fileTags, string fileDescription)
        //{
        //    var tags = fileTags.Split(new[] { '.', ',', ';', '!', '?', ' ' }, StringSplitOptions.RemoveEmptyEntries);

        //    if (tags.Any())
        //    {
        //        fileTags = tags.Aggregate("", (current, tag) => current + (tag.StartsWith("#") ? tag + ", " : "#" + tag + ", "));
        //    }

        //    fileTags = fileTags.Trim(new[] { '.', ',', ';', '!', '?', ' ' });
            
        //    Context.FilesRepository.Attach(file);

        //    file.Tags = fileTags;
        //    file.Description = fileDescription;

        //    Context.Commit();
        //}

        //public List<CommentModel> GetCommentsForFile(int fileId)
        //{
        //    var comments = Context.CommentsRepository.Find(c => c.FileId == fileId && c.IsActive)
        //        .OrderBy(c => c.CommentId)
        //        .ToList();

        //    if (!comments.Any())
        //        return null;

        //    var commentModelList = new List<CommentModel>(comments.Count);
        //    commentModelList.AddRange(comments.Select((c, i) => new CommentModel
        //    {
        //        CommentId = c.CommentId,
        //        Number = (i + 1),
        //        Author = c.User == null ? "Guest" : c.User.UserName,
        //        PublishDate = c.PublishDate,
        //        Text = c.Text
        //    }));

        //    return commentModelList;
        //}

        //public void AddCommentToFile(string commentText, File file, User user)
        //{
        //    var comment = new Comment
        //    {
        //        Text = commentText,
        //        PublishDate = DateTime.Now,
        //        IsActive = true,
        //        FileId = file.FileId,
        //        File = file,
        //        UserId = user != null ? user.UserId : (int?)null,
        //        User = user,
        //    };
        //    Context.CommentsRepository.Add(comment);

        //    Context.Commit();
        //}

        //public void DeleteCommentsFromFile(int fileId, int? commentId)
        //{
        //    if (commentId.HasValue)
        //    {
        //        var comment = Context.CommentsRepository.GetById(commentId.Value);

        //        Context.CommentsRepository.Delete(comment);
        //    }
        //    else
        //    {
        //        var comments = Context.CommentsRepository.Find(c => c.FileId == fileId);

        //        foreach (var comment in comments)
        //        {
        //            Context.CommentsRepository.Delete(comment);
        //        }
        //    }            

        //    Context.Commit();
        //}
    }
}