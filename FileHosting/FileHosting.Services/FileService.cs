using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using FileHosting.Database;
using FileHosting.Database.Models;
using FileHosting.Domain.Enums;
using FileHosting.Domain.Models;
using FileHosting.Services.Extensions;
using File = FileHosting.Database.Models.File;

namespace FileHosting.Services
{
    public class FileService
    {
        private readonly IUnitOfWork _context;

        #region Constructor

        public FileService()
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        #endregion

        #region Files

        public Dictionary<int, string> GetCategoriesDictionary()
        {
            return _context.CategoryRepository.GetAll()
                .ToDictionary(s => s.Id, s => s.Name);
        }        

        public Dictionary<string, int> GetTagsDictionary()
        {
            return _context.TagRepository.GetAll()                               
                .ToDictionary(t => t.Name, t => t.Files.Count);
        }

        public List<FileModel> GetFiles(SearchFilesType searchType, string searchParam, User user)
        {
            var files = new List<File>();
            var fileModels = new List<FileModel>();            

            switch (searchType)
            {
                case SearchFilesType.ByName:
                    files = _context.FileRepository.Find(f => f.Name.ToUpper().Contains(searchParam.ToUpper())).ToList();
                    
                    break;

                case SearchFilesType.ByCategory:
                    int categoriesCount = _context.CategoryRepository.Count();
                    int categoryId;                    
                    if (!int.TryParse(searchParam, out categoryId) || categoryId < 1)
                        categoryId = 1;                    
                    if (categoryId > categoriesCount)
                        categoryId = categoriesCount;
                    
                    files = _context.FileRepository.Find(f => f.Category.Id == categoryId).ToList();
                    
                    break;

                case SearchFilesType.ByTag:
                    files = _context.FileRepository.Find(f => f.Tags.Select(t => t.Name).Contains(searchParam)).ToList();
                    
                    break;                
            }

            if (user == null)
            {
                return files.Where(f => f.IsAllowedAnonymousBrowsing)
                    .Select(f => new FileModel
                    {
                        Id = f.Id,
                        Category = f.Category,
                        Name = f.Name,
                        Size = f.Size,
                        UploadDate = f.UploadDate,
                        IsAllowedAnonymousAction = f.IsAllowedAnonymousAction
                    })
                    .OrderBy(f => f.Id)
                    .ToList();
            }

            foreach (File file in files)
            {
                if (file.AllowedUsers.Any())
                {
                    if (file.AllowedUsers.Contains(user) || file.Owner == user)
                        fileModels.Add(new FileModel
                        {
                            Id = file.Id,
                            Category = file.Category,
                            Name = file.Name,
                            Size = file.Size,
                            UploadDate = file.UploadDate
                        });
                }
                else
                {
                    fileModels.Add(new FileModel
                    {
                        Id = file.Id,
                        Category = file.Category,
                        Name = file.Name,
                        Size = file.Size,
                        UploadDate = file.UploadDate
                    });
                }
            }

            return fileModels;
        }        

        public List<FileModel> GetFilesForUser(int userId)
        {
            return _context.FileRepository.Find(f => f.Owner.Id == userId)
                .Select(f => new FileModel
                {
                    Id = f.Id,
                    Category = f.Category,
                    Name = f.Name,
                    Size = f.Size,
                    UploadDate = f.UploadDate
                })                
                .ToList();
        }

        public File GetFileById(int fileId, User user)
        {
            File file = _context.FileRepository.GetById(fileId);
            if (file == null)
                return null;

            if (user == null)
            {
                return file.IsAllowedAnonymousBrowsing ? file : null;
            }

            if (file.AllowedUsers.Any())
            {
                return file.AllowedUsers.Contains(user) || file.Owner == user ? file : null;
            }

            return file;
        }

        public FileModel GetModelForFile(File file, bool isAuthenticated)
        {
            return isAuthenticated
                ? new FileModel
                {
                    Id = file.Id,
                    Name = file.Name,
                    Category = file.Category,
                    Tags = file.Tags.Any() ? file.Tags.ToTagsString() : "",
                    Description = !string.IsNullOrWhiteSpace(file.Description) ? file.Description : "",
                    UploadDate = file.UploadDate,
                    Size = file.Size,
                    Path = file.Path,
                    Downloads = file.Downloads.ToList(),
                    IsAllowedAnonymousBrowsing = file.IsAllowedAnonymousBrowsing,
                    IsAllowedAnonymousAction = file.IsAllowedAnonymousAction,
                    AllowedUsers = file.AllowedUsers.ToList()
                }
                : new FileModel
                {
                    Id = file.Id,
                    Name = file.Name,
                    Category = file.Category,
                    Tags = file.Tags.Any() ? file.Tags.ToTagsString() : "N/A",
                    Description = string.IsNullOrWhiteSpace(file.Description) ? "N/A" : file.Description,
                    UploadDate = file.UploadDate,
                    Size = file.Size,
                    Path = file.Path,
                    Owner = file.Owner,
                    IsAllowedAnonymousAction = file.IsAllowedAnonymousAction
                };
        }

        public void AddFile(string name, string fullName, string categoryName, long size, string path, User owner, string ipAddress)
        {
            File[] files = _context.FileRepository.Find(f => f.FullName == fullName).ToArray();
            if (files.Any())
            {
                foreach (File file in files)
                {
                    DeleteFile(file, ipAddress);
                }
            }

            Category category = _context.CategoryRepository.First(c => c.Name == categoryName);
            
            _context.FileRepository.Add(new File
            {
                Name = name,
                FullName = fullName,
                Category = category,
                UploadDate = DateTime.UtcNow,
                Size = size,
                Path = path,
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
            string filePath = Path.Combine(ipAddress, file.Path);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            
            _context.FileRepository.Delete(file);

            if (!multiple)
                _context.Commit();            
        }

        public void ChangeFile(File file, string fileTagsString, string newFileDescription, bool allowAnonymousBrowsing,
            bool allowAnonymousAction, string[] allowedUsers)
        {
            _context.FileRepository.Attach(file);

            string[] newFileTags = fileTagsString.ToTagsArray();

            if (newFileTags != null)
            {
                var fileTags = new List<Tag>(newFileTags.Length);
                string[] existingTags = _context.TagRepository.GetAll().Select(t => t.Name).ToArray();

                foreach (string newFileTag in newFileTags)
                {
                    if (existingTags.Contains(newFileTag))
                    {
                        string tag = newFileTag;
                        fileTags.Add(_context.TagRepository.First(t => t.Name == tag));
                    }
                    else
                    {
                        var newTag = new Tag {Name = newFileTag, Files = new List<File>()};

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

            if (allowedUsers != null)
            {
                User[] existingUsers = _context.UserRepository.GetAll().ToArray();

                foreach (User existingUser in existingUsers)
                {
                    if (allowedUsers.Contains(existingUser.Name))
                    {
                        if (!file.AllowedUsers.Contains(existingUser))
                            file.AllowedUsers.Add(existingUser);
                    }
                    else
                    {
                        if (file.AllowedUsers.Contains(existingUser))
                            file.AllowedUsers.Remove(existingUser);
                    }
                }
            }
            else
            {
                file.AllowedUsers.Clear();
            }

            file.Description = newFileDescription;
            file.IsAllowedAnonymousBrowsing = allowAnonymousBrowsing;
            file.IsAllowedAnonymousAction = allowAnonymousAction;

            _context.Commit();
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

        public void ChangeSubscription(File file, User user, bool subscribe)
        {
            _context.FileRepository.Attach(file);

            if (subscribe)
            {
                if (!file.SubscribedUsers.Contains(user))
                    file.SubscribedUsers.Add(user);
            }
            else
            {
                if (file.SubscribedUsers.Contains(user))
                    file.SubscribedUsers.Remove(user);
            }

            _context.Commit();
        }

        #endregion

        #region Comments

        public List<CommentModel> GetCommentsForFile(File file, bool isForFileOwner)
        {
            if (file == null || !file.Comments.Any())
                return null;

            Comment[] comments = isForFileOwner
                ? file.Comments.ToArray()
                : file.Comments.Where(c => c.IsActive).ToArray();

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
            Comment comment = _context.CommentRepository.GetById(commentId);
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
            Comment comment = _context.CommentRepository.GetById(commentId);
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

        #endregion
    }
}