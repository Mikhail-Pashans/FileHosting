using FileHosting.Domain.Enums;
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
using System.Web.Mvc;
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
        public ActionResult Index(int? section, int? page)
        {
            if (User.Identity.IsAuthenticated && Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return RedirectToAction("Login", "Account");

            var fileSectionDictionary = _homeService.GetFileSectionsDictianary();

            var sectionNumber = (section ?? 1);
            if (sectionNumber < 1)
                sectionNumber = 1;
            if (sectionNumber > fileSectionDictionary.Count)
                sectionNumber = fileSectionDictionary.Count;

            var files = _fileService.GetFilesForSection(sectionNumber);

            const int pageSize = 10;

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
                Files = filesPerPages,
                PageInfo = pageInfo,
                FileSectionDictionary = fileSectionDictionary,
                SectionNumber = sectionNumber
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult UserFiles(int? page)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var fileSectionsDictionary = _homeService.GetFileSectionsDictianary();

            var files = _fileService.GetFilesForUser(user.Id);

            const int pageSize = 10;

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
                Files = filesPerPages,
                PageInfo = pageInfo,
                FileSectionsDictionary = fileSectionsDictionary
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult UploadNewFiles(int? page)
        {
            if (!User.Identity.IsAuthenticated && ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name) == null)
                return RedirectToAction("Index", "Home");

            var fileSectionsDictionary = _homeService.GetFileSectionsDictianary();

            var pageNumber = (page ?? 1);

            var viewModel = new UploadNewFilesViewModel
            {
                FileSectionsDictionary = fileSectionsDictionary,
                PageNumber = pageNumber
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadNewFiles(FineUpload upload, string extraParam1, int? extraParam2)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            var owner = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (owner == null)
                return RedirectToAction("Index", "Home");

            var fileName = Path.GetFileNameWithoutExtension(upload.FileName);
            var fileExtension = Path.GetExtension(upload.FileName);
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileExtension))
                return new FineUploaderResult(false, error: "The file or file name is empty!");

            var fullName = fileName + Guid.NewGuid().ToString().Replace("-", "") + fileExtension;

            var fileSectionNumber = int.Parse(upload.FileSection);

            var ipAdress = ConfigurationManager.AppSettings.Get("Server");

            var filePath = string.Format(@"UploadedFiles\{0}\{1}", fileSectionNumber, fullName);

            var pathToSave = Path.Combine(ipAdress, filePath);
            if (System.IO.File.Exists(pathToSave))
                return new FineUploaderResult(false, error: "The same file is already exists!");

            var fileSize = upload.InputStream.Length;

            upload.SaveAs(pathToSave);

            var fileSection = _homeService.GetFileSectionById(fileSectionNumber);

            try
            {
                _fileService.AddFile(fileName + fileExtension, fullName, fileSize, filePath, ipAdress, fileSection, owner);
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
        public ActionResult DownloadFile(int fileId)
        {
            if (User.Identity.IsAuthenticated && Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return RedirectToAction("Login", "Account");

            var file = _fileService.GetFileToDownload(fileId);
            if (file == null)
                return HttpNotFound();

            var ipAdress = ConfigurationManager.AppSettings.Get("Server");
            var pathToFile = Path.Combine(ipAdress, file.Path);

            var totalDownloadAmountLimit = decimal.Parse(ConfigurationManager.AppSettings.Get("TotalDownloadAmountLimit"));
            var totalDownloadSpeedLimit = decimal.Parse(ConfigurationManager.AppSettings.Get("TotalDownloadSpeedLimit"));
            
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
            {
                _fileService.WriteDownload(file, null);
                
                return totalDownloadSpeedLimit != 0
                    ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, "application/octet-stream")
                    : new FilePathResult(pathToFile, "application/octet-stream")
                    {
                        FileDownloadName = file.Name
                    };
            }                                    
            
            if (user.Downloads.Any())
            {
                var downloadsSum = user.Downloads.Sum(d => d.File.Size);
                const decimal devider = 1048576m; // 1024 * 1024
                var downloadAmount = decimal.Round(downloadsSum / devider, 2);

                if (user.DownloadAmountLimit != 0)
                {
                    if (downloadAmount > user.DownloadAmountLimit)
                        return new EmptyResult();
                }
                    
                if (totalDownloadAmountLimit != 0)
                {
                    if (downloadAmount > totalDownloadAmountLimit)
                        return new EmptyResult();
                }

                _fileService.WriteDownload(file, user);
                
                return user.DownloadSpeedLimit != 0
                    ? new FileThrottleResult(pathToFile, file.Name, user.DownloadSpeedLimit, "application/octet-stream")
                    : totalDownloadSpeedLimit != 0
                        ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, "application/octet-stream")
                        : new FilePathResult(pathToFile, "application/octet-stream")
                        {
                            FileDownloadName = file.Name
                        };
            }

            _fileService.WriteDownload(file, user);
            
            return totalDownloadSpeedLimit != 0
                ? new FileThrottleResult(pathToFile, file.Name, totalDownloadSpeedLimit, "application/octet-stream")
                : new FilePathResult(pathToFile, "application/octet-stream")
                {
                    FileDownloadName = file.Name
                };
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult FileDetails(int fileId, int? section, int? page, ViewModelsMessageType? messageType)
        {
            if (User.Identity.IsAuthenticated && Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return RedirectToAction("Login", "Account");

            var file = _fileService.GetFileById(fileId);
            if (file == null)
                return HttpNotFound();

            var sectionNumber = (section ?? 1);

            var pageNumber = (page ?? 1);

            var viewModel = new FileDetailsViewModel
            {
                FileModel = _fileService.GetModelForFile(file, false),
                SectionNumber = sectionNumber,
                PageNumber = pageNumber,
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.A
                            ? "Success! Comment was added."
                            : messageType == ViewModelsMessageType.B
                                ? "Error! Comment was not added."
                                : "Warning! Comment cannot be empty."
                    }
                    : null
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult EditFile(int fileId, int? page, ViewModelsMessageType? messageType)
        {
            var file = _fileService.GetFileById(fileId);
            if (file == null)
                return HttpNotFound();

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (!User.Identity.IsAuthenticated || user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            var pageNumber = (page ?? 1);

            var viewModel = new FileDetailsViewModel
            {
                FileModel = _fileService.GetModelForFile(file, true),
                PageNumber = pageNumber,
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
                    : null
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFile(int fileId, int page, string fileTags, string fileDescription)
        {
            var file = _fileService.GetFileById(fileId);
            if (file == null)
                return HttpNotFound();

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (!User.Identity.IsAuthenticated || user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(fileTags) || string.IsNullOrWhiteSpace(fileDescription))
                return RedirectToAction("EditFile", new { fileId, page, messageType = ViewModelsMessageType.D });
            try
            {
                _fileService.ChangeFile(file, fileTags, fileDescription);
            }
            catch (DataException)
            {
                return RedirectToAction("EditFile", new { fileId, page, messageType = ViewModelsMessageType.B });
            }

            return RedirectToAction("EditFile", new { fileId, page, messageType = ViewModelsMessageType.A });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFile(int fileId, int page)
        {
            var file = _fileService.GetFileById(fileId);
            if (file == null)
                return HttpNotFound();

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (!User.Identity.IsAuthenticated || user == null || !user.Files.Contains(file))
                return RedirectToAction("Index", "Home");

            var ipAdress = ConfigurationManager.AppSettings.Get("Server");

            try
            {
                _fileService.DeleteFile(file, ipAdress, false);
            }
            catch (DataException)
            {
                return RedirectToAction("EditFile", new { fileId, page, messageType = ViewModelsMessageType.C });
            }

            return RedirectToAction("UserFiles", new { page });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetCommentsForFile(int fileId, int? page, ViewModelsMessageType? messageType)
        {
            if (User.Identity.IsAuthenticated && Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return RedirectToAction("Login", "Account");

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            var isFileOwner = User.Identity.IsAuthenticated &&
                user != null &&
                user.Files.Select(f => f.Id).Contains(fileId);

            var comments = isFileOwner
                ? _fileService.GetCommentsForFile(fileId, true)
                : _fileService.GetCommentsForFile(fileId, false);

            const int pageSize = 3;

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

            var commentsPerPages = comments == null
                ? null
                : comments.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new FileCommentsViewModel
            {
                FileId = fileId,
                Comments = commentsPerPages,
                PageInfo = pageInfo,
                IsFileOwner = isFileOwner,
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
                    : null
            };

            return PartialView("_FileCommentsPartial", viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ViewModelsMessageType AddCommentToFile(int fileId, string newCommentText)
        {
            if (User.Identity.IsAuthenticated && Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return ViewModelsMessageType.B;

            var file = _fileService.GetFileById(fileId);
            if (file == null)
                return ViewModelsMessageType.B;

            if (string.IsNullOrWhiteSpace(newCommentText))
                return ViewModelsMessageType.E;

            var user = User.Identity.IsAuthenticated
                ? ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name)
                : null;
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
            var isFileOwner = User.Identity.IsAuthenticated &&
                user != null &&
                user.Files.Select(f => f.Id).Contains(fileId);

            return isFileOwner && _fileService.DeleteCommentFromFile(commentId)
                ? ViewModelsMessageType.C
                : ViewModelsMessageType.D;
        }

        [HttpPost]
        public bool ChangeCommentState(int fileId, int commentId, bool isActive)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            var isFileOwner = User.Identity.IsAuthenticated &&
                user != null &&
                user.Files.Select(f => f.Id).Contains(fileId);

            var result = false;

            if (isFileOwner)
            {
                result = _fileService.ChangeCommentState(commentId, isActive);
            }

            return result;
        }

        #endregion
    }
}