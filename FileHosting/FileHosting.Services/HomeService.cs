using FileHosting.Database;
using FileHosting.Database.Models;
using FileHosting.Domain.Models;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace FileHosting.Services
{
    public class HomeService
    {
        private readonly IUnitOfWork _context;

        public HomeService()
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        public Dictionary<int, string> GetFileSectionsDictianary()
        {
            return _context.SectionRepository
                .GetAll()
                .ToDictionary(s => s.Id, s=> s.Name);
        }

        public SelectList GetFileSectionsSelectList()
        {
            return new SelectList(_context.SectionRepository
                .GetAll()
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(CultureInfo.InvariantCulture)
                }),
                "Value",
                "Text");
        }

        public Section GetFileSectionById(int sectionId)
        {
            return _context.SectionRepository.GetById(sectionId);
        }

        public List<UserModel> GetAllUsersList()
        {
            var usersList = _context.UserRepository.GetAll()
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

        public UserModel GetUserById(int userId)
        {
            var user = _context.UserRepository.Find(u => u.Id == userId)
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

            return user;
        }

        public bool SaveUserChanges(int userId, decimal downloadAmountLimit, decimal downloadSpeedLimit)
        {
            var user = _context.UserRepository.GetById(userId);
            if(user == null)
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

        public void SendEmail(File file, string type)
        {
            if (!file.SubscribedUsers.Any())
                return;
            
            const string email = "jolly.roger.1988@gmail.com";
            const string password = "mike1988";

            var loginInfo = new NetworkCredential(email, password);            
            
            var smtpClient = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = loginInfo
            };

            foreach (var user in file.SubscribedUsers)
            {
                var msg = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = "FileHosting notification",
                    Body = type == "delete" ? string.Format("Dear, {0}, the file \"{1}\" was deleted by its owner.", user.Name, file.Name) : "",
                    IsBodyHtml = true,                    
                };
                msg.To.Add(new MailAddress(user.Email));                

                smtpClient.Send(msg);
            }                                                
        }
    }
}
