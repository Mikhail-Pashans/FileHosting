using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using System.Web.Security;
using FileHosting.Database.Models;
using FileHosting.Domain.Enums;
using FileHosting.Domain.Models;
using FileHosting.MVC.Extensions;
using FileHosting.MVC.Helpers;
using FileHosting.MVC.Infrastructure;
using FileHosting.MVC.Providers;
using FileHosting.MVC.ViewModels;
using FileHosting.Services;
using File = FileHosting.Database.Models.File;

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
        public ActionResult GetCategories()
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            Dictionary<int, string> categoriesDictionary = _fileService.GetCategoriesDictionary();

            return categoriesDictionary.Any() ? PartialView("_CategoriesAjaxPartial", categoriesDictionary) : null;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetTags()
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            Dictionary<string, int> tagsDictionary = _fileService.GetTagsDictionary();

            return tagsDictionary.Any() ? PartialView("_TagsAjaxPartial", tagsDictionary) : null;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int? page, SearchFilesType searchType, string searchParam, string sortOrder)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");
                       
            User user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);

            List<FileModel> files = _fileService.GetFiles(searchType, searchParam, user);

            if (files.Count > 1)
            {
                files = SortFiles(files, sortOrder);
            }            

            var pageSize = int.Parse(ConfigurationManager.AppSettings.Get("FilesPageSize"));

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = files.Count
            };

            int pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            List<FileModel> filesPerPages = files.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();            

            var viewModel = new FilesIndexViewModel
            {
                Files = filesPerPages,
                SearchType = searchType,
                SearchParam = searchParam,                
                IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : string.Empty,
                NameSortParm = sortOrder == "name" ? "name_desc" : "name",
                CategorySortParm = sortOrder == "category" ? "category_desc" : "category",
                SizeSortParm = sortOrder == "size" ? "size_desc" : "size",
                DateSortParm = sortOrder == "size" ? "size_desc" : "size",
                SortOrder = sortOrder,                
                IsAuthenticated = user != null,                
                PageInfo = pageInfo
            };

            return View(viewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult FileDetails(int fileId, string returnUrl, ViewModelsMessageType? messageType)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            User user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);

            File file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                NameValueCollection queryString = Request.QueryString.ToEditable();
                queryString.Remove("fileId");
                returnUrl = Url.Action("Index") + "?" + queryString;
            }            

            var viewModel = new FileDetailsViewModel
            {
                FileModel = _fileService.GetModelForFile(file, isAuthenticated: false),
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
                ReturnUrl = returnUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult DownloadFile(int fileId, string returnUrl)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            User user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);

            File file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                NameValueCollection queryString = Request.QueryString.ToEditable();
                queryString.Remove("fileId");
                returnUrl = Url.Action("Index") + "?" + queryString;
            }

            string ipAdress = ConfigurationManager.AppSettings.Get("Server");
            string pathToFile = Path.Combine(ipAdress, file.Path);

            var totalDownloadAmountLimit =
                decimal.Parse(ConfigurationManager.AppSettings.Get("TotalDownloadAmountLimit"));
            var totalDownloadSpeedLimit =
                decimal.Parse(ConfigurationManager.AppSettings.Get("TotalDownloadSpeedLimit"));
            const decimal divider = 1048576; // 1024 * 1024
            decimal currentDownloadAmount;

            if (user == null)
            {
                if (totalDownloadAmountLimit != 0)
                {
                    string userHostAddress = HttpContext.Request.UserHostAddress;

                    currentDownloadAmount = Convert.ToDecimal(Session[userHostAddress] ?? 0);
                    currentDownloadAmount += decimal.Round(file.Size / divider, 2);

                    if (currentDownloadAmount > totalDownloadAmountLimit)
                        return RedirectToAction("FileDetails", new { fileId, returnUrl, messageType = ViewModelsMessageType.D });

                    _fileService.WriteDownload(file, null);
                    Session[userHostAddress] = currentDownloadAmount;

                    return totalDownloadSpeedLimit != 0
                        ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, MediaTypeNames.Application.Octet)
                        : File(pathToFile, MediaTypeNames.Application.Octet, file.Name);
                }

                _fileService.WriteDownload(file, null);

                return totalDownloadSpeedLimit != 0
                    ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, MediaTypeNames.Application.Octet)
                    : File(pathToFile, MediaTypeNames.Application.Octet, file.Name);
            }

            if (user.Downloads.Any())
            {
                long downloadsSum = user.Downloads.Sum(d => d.File.Size);
                currentDownloadAmount = decimal.Round(downloadsSum / divider, 2);

                if (user.DownloadAmountLimit != 0)
                {
                    if (currentDownloadAmount > user.DownloadAmountLimit)
                        return RedirectToAction("FileDetails", new { fileId, returnUrl, messageType = ViewModelsMessageType.D });
                }

                if (totalDownloadAmountLimit != 0)
                {
                    if (currentDownloadAmount > totalDownloadAmountLimit)
                        return RedirectToAction("FileDetails", new { fileId, returnUrl, messageType = ViewModelsMessageType.D });
                }

                _fileService.WriteDownload(file, user);

                return user.DownloadSpeedLimit != 0
                    ? new FileThrottleResult(pathToFile, file.Name, user.DownloadSpeedLimit, MediaTypeNames.Application.Octet)
                    : totalDownloadSpeedLimit != 0
                        ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, MediaTypeNames.Application.Octet)
                        : File(pathToFile, MediaTypeNames.Application.Octet, file.Name);
            }

            if (totalDownloadAmountLimit != 0)
            {
                currentDownloadAmount = decimal.Round(file.Size / divider, 2);

                if (currentDownloadAmount > totalDownloadAmountLimit)
                    return RedirectToAction("FileDetails", new { fileId, returnUrl, messageType = ViewModelsMessageType.D });
            }

            _fileService.WriteDownload(file, user);

            return totalDownloadSpeedLimit != 0
                ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, MediaTypeNames.Application.Octet)
                : File(pathToFile, MediaTypeNames.Application.Octet, file.Name);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSubscription(int fileId, string returnUrl, SubscribeActionType subscription)
        {
            User user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);

            File file = _fileService.GetFileById(fileId, user);
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
                return RedirectToAction("FileDetails",
                    new
                    {
                        fileId,                        
                        returnUrl,
                        messageType =
                            subscription == SubscribeActionType.Subscribe
                                ? ViewModelsMessageType.F
                                : ViewModelsMessageType.E                        
                    });
            }

            return RedirectToAction("FileDetails",
                new
                {
                    fileId,                    
                    returnUrl,
                    messageType =
                        subscription == SubscribeActionType.Subscribe
                            ? ViewModelsMessageType.E
                            : ViewModelsMessageType.F                    
                });
        }

        [HttpGet]
        public ActionResult UserFiles(int? page, string sortOrder)
        {
            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            List<FileModel> files = _fileService.GetFilesForUser(user.Id);
            if (files.Count > 1)
            {
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
                    case "category":
                        files = files.OrderBy(f => f.Category).ToList();
                        break;
                    case "category_desc":
                        files = files.OrderByDescending(f => f.Category).ToList();
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
            }            

            int pageSize = int.Parse(ConfigurationManager.AppSettings.Get("FilesPageSize"));

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = files.Count
            };

            int pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            List<FileModel> filesPerPages = files.Skip((pageNumber - 1)*pageSize).Take(pageSize).ToList();
            string currentSort = sortOrder;
            string idSortParm = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            string nameSortParm = sortOrder == "name" ? "name_desc" : "name";
            string categorySortParm = sortOrder == "section" ? "section_desc" : "section";
            string sizeSortParm = sortOrder == "size" ? "size_desc" : "size";
            string dateSortParm = sortOrder == "date" ? "date_desc" : "date";

            var viewModel = new UserFilesViewModel
            {
                Files = filesPerPages,
                IdSortParm = idSortParm,
                NameSortParm = nameSortParm,
                CategorySortParm = categorySortParm,
                SizeSortParm = sizeSortParm,
                DateSortParm = dateSortParm,
                CurrentSort = currentSort,                
                PageInfo = pageInfo
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult UploadNewFiles(string sortOrder, string currentFilter, int? page)
        {
            if (((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name) == null)
                return RedirectToAction("Index", "Home");

            Dictionary<int, string> categoriesDictionary = _fileService.GetCategoriesDictionary();

            int pageNumber = (page ?? 1);

            var viewModel = new UploadNewFilesViewModel
            {
                CategoriesDictionary = categoriesDictionary,
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
            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            string fileName = Path.GetFileNameWithoutExtension(upload.FileName);
            string fileExtension = Path.GetExtension(upload.FileName);
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileExtension))
                return new FineUploaderResult(false, error: "The file or file name is empty!");

            string fullName = fileName + Guid.NewGuid().ToString().Replace("-", "") + fileExtension;

            string ipAddress = ConfigurationManager.AppSettings.Get("Server");

            string filePath = string.Format("UploadedFiles/{0}/{1}", upload.FileCategory, fullName);

            string pathToSave = Path.Combine(ipAddress, filePath);
            if (System.IO.File.Exists(pathToSave))
                return new FineUploaderResult(false, error: "The same file is already exists!");

            long fileSize = upload.InputStream.Length;

            upload.SaveAs(pathToSave);            

            try
            {
                _fileService.AddFile(fileName + fileExtension, fullName, upload.FileCategory, fileSize, filePath, user, ipAddress);
            }
            catch (DataException ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }

            return new FineUploaderResult(true, new {message = "Upload is finished successfully!"});
        }        

        [HttpGet]
        public ActionResult EditFile(int fileId, string sortOrder, string currentFilter, int? page,
            ViewModelsMessageType? messageType)
        {
            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);

            File file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            if (user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            int pageNumber = (page ?? 1);

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
                //CurrentSort = sortOrder,
                //CurrentFilter = currentFilter,
                //PageNumber = pageNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFile(int fileId, string sortOrder, string currentFilter, int page, string fileTags,
            string fileDescription, FileBrowsingPermission browsingPermission, bool allowAnonymousAction,
            string[] allowedUsers)
        {
            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);

            File file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            if (user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(fileTags) || string.IsNullOrWhiteSpace(fileDescription))
                return RedirectToAction("EditFile",
                    new {fileId, sortOrder, currentFilter, page, messageType = ViewModelsMessageType.D});

            string fileName = file.Name;
            string fileOwner = file.Owner.Name;
            string fileSection = file.Category.Name;
            ICollection<User> subscribedUsers = file.SubscribedUsers;

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
                return RedirectToAction("EditFile",
                    new {fileId, sortOrder, currentFilter, page, messageType = ViewModelsMessageType.B});
            }

            if (subscribedUsers.Any())
                _homeService.SendEmail(EmailType.FileChanged, subscribedUsers, fileName, fileOwner, fileSection);

            return RedirectToAction("EditFile",
                new {fileId, sortOrder, currentFilter, page, messageType = ViewModelsMessageType.A});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFile(int fileId, int page)
        {
            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);

            File file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            if (user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            string ipAdress = ConfigurationManager.AppSettings.Get("Server");
            string fileName = file.Name;
            string fileOwner = file.Owner.Name;
            string fileSection = file.Category.Name;
            ICollection<User> subscribedUsers = file.SubscribedUsers;

            try
            {
                _fileService.DeleteFile(file, ipAdress, false);
            }
            catch (DataException)
            {
                return RedirectToAction("EditFile", new {fileId, page, messageType = ViewModelsMessageType.C});
            }

            if (subscribedUsers.Any())
                _homeService.SendEmail(EmailType.FileDeleted, subscribedUsers, fileName, fileOwner, fileSection);

            return RedirectToAction("UserFiles", new {page});
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetCommentsForFile(int fileId, int? page, ViewModelsMessageType? messageType)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);

            File file = _fileService.GetFileById(fileId, user);
            if (file == null)
                return HttpNotFound();

            bool isFileOwner = user != null && user.Files.Contains(file);

            List<CommentModel> comments = _fileService.GetCommentsForFile(file, isFileOwner);

            int pageSize = int.Parse(ConfigurationManager.AppSettings.Get("CommentsPageSize"));

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = comments == null ? 0 : comments.Count
            };

            int pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            List<CommentModel> commentsPerPages = comments != null
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

            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);

            File file = _fileService.GetFileById(fileId, user);
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
            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);

            bool isFileOwner = user != null && user.Files.Select(f => f.Id).Contains(fileId);

            return isFileOwner && _fileService.DeleteCommentFromFile(commentId)
                ? ViewModelsMessageType.C
                : ViewModelsMessageType.D;
        }

        [HttpPost]
        public bool ChangeCommentState(int fileId, int commentId, bool isActive)
        {
            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);

            bool isFileOwner = user != null && user.Files.Select(f => f.Id).Contains(fileId);

            return isFileOwner && _fileService.ChangeCommentState(commentId, isActive);
        }

        #endregion

        #region Private methods

        private List<FileModel> SortFiles(List<FileModel> files, string sortOrder)
        {
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
                case "category":
                    files = files.OrderBy(f => f.Category).ToList();
                    break;
                case "category_desc":
                    files = files.OrderByDescending(f => f.Category).ToList();
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

            return files;
        }

        #endregion
    }
}