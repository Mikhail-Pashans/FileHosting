﻿using FileHosting.Domain.Enums;
using FileHosting.MVC.Helpers;
using FileHosting.MVC.Infrastructure;
using FileHosting.MVC.Providers;
using FileHosting.MVC.ViewModels;
using FileHosting.Services;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Providers.Entities;
using System.Web.Security;

namespace FileHosting.MVC.Controllers
{
    [Authorize(Roles = "Administrator, Moderator, RegisteredUser")]
    public class FilesController : Controller
    {
        private readonly FileService _fileService;
        private readonly HomeService _homeService;        

        #region Constructor

        public FilesController()
        {
            _fileService = new FileService();
            _homeService = new HomeService();            
        }

        #endregion

        #region Actions

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? section, int? page)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);            

            var fileSectionDictionary = _homeService.GetFileSectionsDictianary();
            
            var sectionNumber = (section ?? 1);
            if (sectionNumber < 1)
                sectionNumber = 1;
            if (sectionNumber > fileSectionDictionary.Count)
                sectionNumber = fileSectionDictionary.Count;

            var files = _fileService.GetFilesForSection(sectionNumber, user);

            var currentSort = sortOrder;
            var idSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            var nameSortParm = sortOrder == "name" ? "name_desc" : "name";
            var sectionSortParm = sortOrder == "section" ? "section_desc" : "section";
            var sizeSortParm = sortOrder == "size" ? "size_desc" : "size";
            var dateSortParm = sortOrder == "date" ? "date_desc" : "date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            currentFilter = searchString;                       

            if (!String.IsNullOrEmpty(searchString))
            {
                files = files.Where(f => f.Name.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            switch (sortOrder)
            {                
                case "id_desc":
                    files = files.OrderByDescending(f => f.Id).ToList();
                    break;
                case "name":
                    files = files.OrderBy(f => f.Name).ToList();
                    break;
                case "name_desc":
                    files = files.OrderByDescending(f => f.Name).ToList();
                    break;
                case "section":
                    files = files.OrderBy(f => f.Section).ToList();
                    break;
                case "section_desc":
                    files = files.OrderByDescending(f => f.Section).ToList();
                    break;
                case "size":
                    files = files.OrderBy(f => f.Size).ToList();
                    break;
                case "size_desc":
                    files = files.OrderByDescending(f => f.Size).ToList();
                    break;
                case "date":
                    files = files.OrderBy(f => f.UploadDate).ToList();
                    break;
                case "date_desc":
                    files = files.OrderByDescending(f => f.UploadDate).ToList();
                    break;
                default:
                    files = files.OrderBy(f => f.Id).ToList();                    
                    break;
            }                                               

            var pageSize = int.Parse(ConfigurationManager.AppSettings.Get("FilesPageSize"));

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = files.Count
            };

            var pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            var filesPerPages = files.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new FilesIndexViewModel
            {
                FileSectionDictionary = fileSectionDictionary,
                Files = filesPerPages,
                IdSortParm = idSortParm,
                NameSortParm = nameSortParm,
                SectionSortParm = sectionSortParm,
                SizeSortParm = sizeSortParm,
                DateSortParm = dateSortParm,
                CurrentSort = currentSort,
                CurrentFilter = currentFilter,
                IsAuthenticated = user != null,
                SectionNumber = sectionNumber,                
                PageInfo = pageInfo
            };

            return View(viewModel);
        }        

        [HttpGet]
        public ActionResult UserFiles(string sortOrder, string currentFilter, string searchString, int? page)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var currentSort = sortOrder;
            var idSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            var nameSortParm = sortOrder == "name" ? "name_desc" : "name";
            var sectionSortParm = sortOrder == "section" ? "section_desc" : "section";
            var sizeSortParm = sortOrder == "size" ? "size_desc" : "size";
            var dateSortParm = sortOrder == "date" ? "date_desc" : "date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            currentFilter = searchString;

            var files = _fileService.GetFilesForUser(user.Id);

            if (!String.IsNullOrEmpty(searchString))
            {
                files = files.Where(f => f.Name.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            switch (sortOrder)
            {                
                case "id_desc":
                    files = files.OrderByDescending(f => f.Id).ToList();
                    break;
                case "name":
                    files = files.OrderBy(f => f.Name).ToList();
                    break;                                
                case "name_desc":
                    files = files.OrderByDescending(f => f.Name).ToList();
                    break;
                case "section":
                    files = files.OrderBy(f => f.Section).ToList();
                    break;
                case "section_desc":
                    files = files.OrderByDescending(f => f.Section).ToList();
                    break;
                case "size":
                    files = files.OrderBy(f => f.Size).ToList();
                    break;
                case "size_desc":
                    files = files.OrderByDescending(f => f.Size).ToList();
                    break;
                case "date":
                    files = files.OrderBy(f => f.UploadDate).ToList();
                    break;
                case "date_desc":
                    files = files.OrderByDescending(f => f.UploadDate).ToList();
                    break;
                default:
                    files = files.OrderBy(f => f.Id).ToList();
                    break;
            }            

            var fileSectionsDictionary = _homeService.GetFileSectionsDictianary();

            var pageSize = int.Parse(ConfigurationManager.AppSettings.Get("FilesPageSize"));

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = files.Count
            };

            var pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            var filesPerPages = files.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new UserFilesViewModel
            {
                FileSectionsDictionary = fileSectionsDictionary,
                Files = filesPerPages,
                IdSortParm = idSortParm,
                NameSortParm = nameSortParm,
                SectionSortParm = sectionSortParm,
                SizeSortParm = sizeSortParm,
                DateSortParm = dateSortParm,
                CurrentSort = currentSort,
                CurrentFilter = currentFilter,
                PageInfo = pageInfo
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult UploadNewFiles(string sortOrder, string currentFilter, int? page)
        {
            if (((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name) == null)
                return RedirectToAction("Index", "Home");

            var fileSectionsDictionary = _homeService.GetFileSectionsDictianary();

            var pageNumber = (page ?? 1);

            var viewModel = new UploadNewFilesViewModel
            {
                FileSectionsDictionary = fileSectionsDictionary,
                CurrentFilter = currentFilter,
                CurrentSort = sortOrder,                
                PageNumber = pageNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadNewFiles(FineUpload upload, string extraParam1, int? extraParam2)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var fileName = Path.GetFileNameWithoutExtension(upload.FileName);
            var fileExtension = Path.GetExtension(upload.FileName);
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileExtension))
                return new FineUploaderResult(false, error: "The file or file name is empty!");

            var fullName = fileName + Guid.NewGuid().ToString().Replace("-", "") + fileExtension;

            var fileSectionNumber = int.Parse(upload.FileSection);

            var ipAddress = ConfigurationManager.AppSettings.Get("Server");

            var filePath = string.Format("UploadedFiles/{0}/{1}", fileSectionNumber, fullName);

            var pathToSave = Path.Combine(ipAddress, filePath);
            if (System.IO.File.Exists(pathToSave))
                return new FineUploaderResult(false, error: "The same file is already exists!");

            var fileSize = upload.InputStream.Length;

            upload.SaveAs(pathToSave);

            var fileSection = _homeService.GetFileSectionById(fileSectionNumber);

            try
            {
                _fileService.AddFile(fileName + fileExtension, fullName, fileSize, filePath, ipAddress, fileSection, user);
            }
            catch (DataException ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }

            return new FineUploaderResult(true, new { message = "Upload is finished successfully!" });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadFile(int fileId, int section, int page)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);

            var file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            var ipAdress = ConfigurationManager.AppSettings.Get("Server");
            var pathToFile = Path.Combine(ipAdress, file.Path);

            var totalDownloadAmountLimit = decimal.Parse(ConfigurationManager.AppSettings.Get("TotalDownloadAmountLimit"));
            var totalDownloadSpeedLimit = decimal.Parse(ConfigurationManager.AppSettings.Get("TotalDownloadSpeedLimit"));
            const decimal divider = 1048576; // 1024 * 1024
            decimal currentDownloadAmount;

            if (user == null)
            {
                if (totalDownloadAmountLimit != 0)
                {
                    var userHostAddress = HttpContext.Request.UserHostAddress;

                    currentDownloadAmount = Convert.ToDecimal(Session[userHostAddress] ?? 0);
                    currentDownloadAmount += decimal.Round(file.Size / divider, 2);

                    if (currentDownloadAmount > totalDownloadAmountLimit)
                        return RedirectToAction("FileDetails", new { fileId, section, page, messageType = ViewModelsMessageType.D });

                    _fileService.WriteDownload(file, null);
                    Session[userHostAddress] = currentDownloadAmount;

                    return totalDownloadSpeedLimit != 0
                        ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, System.Net.Mime.MediaTypeNames.Application.Octet)
                        : File(pathToFile, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
                }

                _fileService.WriteDownload(file, null);

                return totalDownloadSpeedLimit != 0
                    ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, System.Net.Mime.MediaTypeNames.Application.Octet)
                    : File(pathToFile, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
            }

            if (user.Downloads.Any())
            {
                var downloadsSum = user.Downloads.Sum(d => d.File.Size);
                currentDownloadAmount = decimal.Round(downloadsSum / divider, 2);

                if (user.DownloadAmountLimit != 0)
                {
                    if (currentDownloadAmount > user.DownloadAmountLimit)
                        return RedirectToAction("FileDetails", new { fileId, section, page, messageType = ViewModelsMessageType.D });
                }

                if (totalDownloadAmountLimit != 0)
                {
                    if (currentDownloadAmount > totalDownloadAmountLimit)
                        return RedirectToAction("FileDetails", new { fileId, section, page, messageType = ViewModelsMessageType.D });
                }

                _fileService.WriteDownload(file, user);

                return user.DownloadSpeedLimit != 0
                    ? new FileThrottleResult(pathToFile, file.Name, user.DownloadSpeedLimit, System.Net.Mime.MediaTypeNames.Application.Octet)
                    : totalDownloadSpeedLimit != 0
                        ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, System.Net.Mime.MediaTypeNames.Application.Octet)
                        : File(pathToFile, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
            }

            if (totalDownloadAmountLimit != 0)
            {
                currentDownloadAmount = decimal.Round(file.Size / divider, 2);

                if (currentDownloadAmount > totalDownloadAmountLimit)
                    return RedirectToAction("FileDetails", new { fileId, section, page, messageType = ViewModelsMessageType.D });
            }

            _fileService.WriteDownload(file, user);

            return totalDownloadSpeedLimit != 0
                ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, System.Net.Mime.MediaTypeNames.Application.Octet)
                : File(pathToFile, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult FileDetails(int fileId, string sortOrder, string currentFilter, int? section, int? page, ViewModelsMessageType? messageType)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            
            var file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            var sectionNumber = (section ?? 1);

            var pageNumber = (page ?? 1);

            var viewModel = new FileDetailsViewModel
            {
                FileModel = _fileService.GetModelForFile(file, false),                
                IsAuthenticated = user != null,
                IsSubscribed = user != null && file.SubscribedUsers.Contains(user),
                IsOwner = user != null && file.Owner == user,
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.A
                            ? "Success! Comment was added."
                            : messageType == ViewModelsMessageType.B
                                ? "Error! Comment was not added."
                                : messageType == ViewModelsMessageType.C
                                    ? "Warning! Comment cannot be empty."
                                    : messageType == ViewModelsMessageType.D
                                        ? "Warning! You have exceeded the download amount limit. File will not be downloaded."
                                        : messageType == ViewModelsMessageType.E
                                            ? "Success! You are subscribed to file changes."
                                            : "Succes! You are not subscribed to file changes."
                    }
                    : null,
                CurrentSort = sortOrder,
                CurrentFilter = currentFilter,
                SectionNumber = sectionNumber,
                PageNumber = pageNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSubscription(int fileId, int page, SubscribeActionType subscription)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            
            var file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();
            
            if (user == null || user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            try
            {
                _fileService.ChangeSubscription(file, user, subscription == SubscribeActionType.Subscribe);
            }
            catch (DataException)
            {
                return RedirectToAction("FileDetails", new { fileId, page, messageType = subscription == SubscribeActionType.Subscribe ? ViewModelsMessageType.F : ViewModelsMessageType.E });
            }

            return RedirectToAction("FileDetails", new { fileId, page, messageType = subscription == SubscribeActionType.Subscribe ? ViewModelsMessageType.E : ViewModelsMessageType.F });
        }

        [HttpGet]
        public ActionResult EditFile(int fileId, string sortOrder, string currentFilter, int? page, ViewModelsMessageType? messageType)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            
            var file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            if (user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            var pageNumber = (page ?? 1);

            var viewModel = new FileDetailsViewModel
            {
                FileModel = _fileService.GetModelForFile(file, true),
                Users = _homeService.GetAllUsersList(),
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.A
                            ? "Success! File changes were saved."
                            : messageType == ViewModelsMessageType.B
                                ? "Error! File changes were not saved."
                                : messageType == ViewModelsMessageType.C
                                    ? "Error! File was not deleted."
                                    : "Warning! Tags and description cannot be empty."
                    }
                    : null,
                CurrentSort = sortOrder,
                CurrentFilter = currentFilter,
                PageNumber = pageNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFile(int fileId, string sortOrder, string currentFilter, int page, string fileTags, string fileDescription, FileBrowsingPermission browsingPermission, bool allowAnonymousAction, string[] allowedUsers)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            
            var file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            if (user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(fileTags) || string.IsNullOrWhiteSpace(fileDescription))
                return RedirectToAction("EditFile", new { fileId, sortOrder, currentFilter, page, messageType = ViewModelsMessageType.D });

            var fileName = file.Name;            
            var fileOwner = file.Owner.Name;
            var fileSection = file.Section.Name;
            var subscribedUsers = file.SubscribedUsers;

            try
            {
                switch (browsingPermission)
                {
                    case FileBrowsingPermission.AllUsers:
                        _fileService.ChangeFile(file, fileTags, fileDescription, true, allowAnonymousAction, null);
                        break;

                    case FileBrowsingPermission.RegisteredUsers:
                        _fileService.ChangeFile(file, fileTags, fileDescription, false, false, null);
                        break;

                    case FileBrowsingPermission.SpecificUsers:
                        _fileService.ChangeFile(file, fileTags, fileDescription, false, false, allowedUsers);
                        break;
                }                
            }
            catch (DataException)
            {
                return RedirectToAction("EditFile", new { fileId, sortOrder, currentFilter, page, messageType = ViewModelsMessageType.B });
            }

            if (subscribedUsers.Any())
                _homeService.SendEmail(EmailType.FileChanged, subscribedUsers, fileName, fileOwner, fileSection);

            return RedirectToAction("EditFile", new { fileId, sortOrder, currentFilter, page, messageType = ViewModelsMessageType.A });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFile(int fileId, int page)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            
            var file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            if (user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            var ipAdress = ConfigurationManager.AppSettings.Get("Server");
            var fileName = file.Name;
            var fileOwner = file.Owner.Name;
            var fileSection = file.Section.Name;
            var subscribedUsers = file.SubscribedUsers;

            try
            {
                _fileService.DeleteFile(file, ipAdress, false);
            }
            catch (DataException)
            {
                return RedirectToAction("EditFile", new { fileId, page, messageType = ViewModelsMessageType.C });
            }            
            
            if (subscribedUsers.Any())
                _homeService.SendEmail(EmailType.FileDeleted, subscribedUsers, fileName, fileOwner, fileSection);
            
            return RedirectToAction("UserFiles", new { page });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetCommentsForFile(int fileId, int? page, ViewModelsMessageType? messageType)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);

            var file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            var isFileOwner = user != null && user.Files.Contains(file);

            var comments = _fileService.GetCommentsForFile(file, isFileOwner);

            var pageSize = int.Parse(ConfigurationManager.AppSettings.Get("CommentsPageSize"));

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = comments == null ? 0 : comments.Count
            };

            var pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            var commentsPerPages = comments != null
                ? comments.Skip((pageNumber - 1)*pageSize)
                    .Take(pageSize)
                    .ToList()
                : null;

            var viewModel = new FileCommentsViewModel
            {
                FileId = fileId,
                Comments = commentsPerPages,
                IsFileOwner = isFileOwner,
                IsAuthenticated = user != null,
                IsAllowedAnonymousAction = file.IsAllowedAnonymousAction,
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.A
                            ? "Success! Comment was added."
                            : messageType == ViewModelsMessageType.B
                                ? "Error! Comment was not added."
                                : messageType == ViewModelsMessageType.C
                                    ? "Success! Comment was deleted."
                                    : messageType == ViewModelsMessageType.D
                                        ? "Error! Comment was not deleted."
                                        : "Warning! Comment cannot be empty."
                    }
                    : null,
                PageInfo = pageInfo
            };

            return PartialView("_FileCommentsPartial", viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ViewModelsMessageType AddCommentToFile(int fileId, string newCommentText)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return ViewModelsMessageType.B;

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            
            var file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return ViewModelsMessageType.B;

            if (string.IsNullOrWhiteSpace(newCommentText))
                return ViewModelsMessageType.E;

            try
            {
                _fileService.AddCommentToFile(newCommentText, file, user);
            }
            catch (DataException)
            {
                return ViewModelsMessageType.B;
            }

            return ViewModelsMessageType.A;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewModelsMessageType DeleteCommentFromFile(int fileId, int commentId)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            
            var isFileOwner = user != null && user.Files.Select(f => f.Id).Contains(fileId);

            return isFileOwner && _fileService.DeleteCommentFromFile(commentId)
                ? ViewModelsMessageType.C
                : ViewModelsMessageType.D;
        }

        [HttpPost]
        public bool ChangeCommentState(int fileId, int commentId, bool isActive)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            
            var isFileOwner = user != null && user.Files.Select(f => f.Id).Contains(fileId);
           
            return isFileOwner && _fileService.ChangeCommentState(commentId, isActive);
        }

        #endregion
    }
}