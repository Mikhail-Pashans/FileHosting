﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Mvc;
using FileHosting.Database;
using FileHosting.Database.Models;
using FileHosting.Domain.Enums;
using FileHosting.Domain.Models;

namespace FileHosting.Services
{
    public class HomeService
    {
        private readonly IUnitOfWork _context;

        #region Constructor

        public HomeService()
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        #endregion

        #region Users methods

        public List<UserModel> GetAllUsersList()
        {
            List<UserModel> usersList = _context.UserRepository.GetAll()
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    CreationDate = u.CreationDate
                })
                .ToList();

            return usersList;
        }

        public UserModel GetModelForUser(int userId)
        {
            return _context.UserRepository.Find(u => u.Id == userId)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    CreationDate = u.CreationDate,
                    DownloadAmountLimit = u.DownloadAmountLimit,
                    DownloadSpeedLimit = u.DownloadSpeedLimit,
                    Roles = u.Roles.ToList()
                })
                .FirstOrDefault();
        }

        public User GetUserById(int userId)
        {
            return _context.UserRepository.GetById(userId);
        }

        public bool SaveUserChanges(int userId, decimal downloadAmountLimit, decimal downloadSpeedLimit)
        {
            User user = _context.UserRepository.GetById(userId);
            if (user == null)
                return false;

            _context.UserRepository.Attach(user);

            user.DownloadAmountLimit = downloadAmountLimit;
            user.DownloadSpeedLimit = downloadSpeedLimit;

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

        #region News methods

        public List<NewsModel> GetActiveNews()
        {
            return _context.NewsRepository.Find(n => n.IsActive)
                .Select(n => new NewsModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Text = n.Text,
                    Picture = n.Picture,
                    PublishDate = n.PublishDate
                })
                .OrderByDescending(n => n.PublishDate)
                .ToList();
        }

        public News GetNewsById(int newsId)
        {
            return _context.NewsRepository.GetById(newsId);
        }

        public NewsModel GetModelForNews(int newsId)
        {
            News news = _context.NewsRepository.GetById(newsId);
            if (news == null)
                return null;

            return new NewsModel
            {
                Id = news.Id,
                Name = news.Name,
                Text = news.Text,
                Picture = news.Picture,
                PublishDate = news.PublishDate
            };
        }

        public void AddNews(string newsName, string newsText, string picturePath, User author)
        {
            _context.NewsRepository.Add(new News
            {
                Name = newsName,
                Text = newsText,
                Picture = picturePath,
                PublishDate = DateTime.UtcNow,
                IsActive = true,
                Author = author
            });

            _context.Commit();
        }

        public void ChangeNews(News news, string newsName, string newsText, string newPicturePath)
        {
            _context.NewsRepository.Attach(news);

            news.Name = newsName;
            news.Text = newsText;

            if (newPicturePath != null)
                news.Picture = newPicturePath;

            _context.Commit();
        }

        #endregion

        #region Common methods        

        public Task SendEmail(EmailType emailType, IEnumerable<User> recipients, string fileName = null,
            string fileOwner = null, string fileSection = null)
        {
            return Task.Factory.StartNew(() =>
            {
                var smtpClient = new SmtpClient();
                var body = string.Empty;

                foreach (User recipient in recipients)
                {
                    if (emailType == EmailType.FileDeleted)
                    {
                        body = string.Format(
                            "<p>Dear <strong>{0}</strong>!</p><p>The user <strong>{1}</strong> has deleted the file <strong>{2}</strong> in the <strong>{3}</strong> section.</p>",
                            recipient.Name, fileOwner, fileName, fileSection);
                    }
                    if (emailType == EmailType.FileDeleted)
                    {
                        body = string.Format(
                            "<p>Dear <strong>{0}</strong>!</p><p>The user <strong>{1}</strong> has deleted the file <strong>{2}</strong> in the <strong>{3}</strong> section.</p>",
                            recipient.Name, fileOwner, fileName, fileSection);
                    }
                    if (emailType == EmailType.UserPasswordChanged)
                    {
                        body = string.Format(
                            "<p>Dear <strong>{0}</strong>!</p><p>Your password was changed by the site administrator.</p><p>If you have any questions, please contact the site administrator.</p>",
                            recipient.Name);
                    }
                    var msg = new MailMessage
                    {                    
                        Subject = "FileHosting notification",
                        Body = body,
                        IsBodyHtml = true,
                    };
                    msg.To.Add(new MailAddress(recipient.Email));

                    smtpClient.Send(msg);
                }
            });
        }        

        #endregion
    }
}