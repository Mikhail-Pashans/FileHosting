using FileHosting.Domain.Enums;
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
        public ActionResult Index(int? section, int? page)
        {
            var fileSectionDictionary = _homeService.GetFileSectionDictianary();
            
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
            
            var fileSectionsDictionary = _homeService.GetFileSectionDictianary();
            
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

            var fileSectionSelectList = _homeService.GetFileSectionSelectList();
            
            var pageNumber = (page ?? 1);

            var viewModel = new UploadNewFilesViewModel
            {
                FileSectionSelectList = fileSectionSelectList,
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

            var ipAdress = ConfigurationManager.AppSettings["Server"];

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

        public ActionResult DownloadFile(int fileId)
        {
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);

            var file = _fileService.GetFileToDownload(fileId, user);
            if (file == null)
                return HttpNotFound();            

            var ipAdress = ConfigurationManager.AppSettings["Server"];
            var pathToDonload = Path.Combine(ipAdress, file.Path);

            return File(pathToDonload, System.Net.Mime.MediaTypeNames.Application.Octet, file.Name);
        }

        public ActionResult FileDetails(int fileId, int? section, int? page, ViewModelsMessageType? messageType)
        {
            var file = _fileService.GetFileById(fileId);
            if (file == null)
                return HttpNotFound();

            var sectionNumber = (section ?? 1);

            var pageNumber = (page ?? 1);

            FileDetailsViewModel viewModel;            

            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);

            if (!User.Identity.IsAuthenticated && user == null || !user.Files.Contains(file))
            {
                viewModel = new FileDetailsViewModel
                {
                    FileModel = _fileService.GetModelForFile(file, false),
                    Section = sectionNumber,
                    PageNumber = pageNumber,
                    Message = messageType.HasValue
                        ? new Message
                        {
                            MessageType = messageType.Value,
                            MessageText = messageType == ViewModelsMessageType.Default
                                ? "Error! A comment cannot be empty."
                                : messageType == ViewModelsMessageType.Error
                                    ? "Error! The comment was not added."
                                    : "Success! The comment was added."
                        }
                        : null
                };

                return View(viewModel);
            }

            viewModel = new FileDetailsViewModel
            {
                FileModel = _fileService.GetModelForFile(file, true),
                PageNumber = pageNumber,
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.Default
                            ? "Error! A description and tags cannot be empty."
                            : messageType == ViewModelsMessageType.Error
                                ? "Error! The file changing or deleting failed."
                                : "Success! The file was changed."
                    }
                    : null
            };

            return View("ChangeFile", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeFile(FileActionsType type, int fileId, int page, string fileTags = null, string fileDescription = null)
        {
            var file = _fileService.GetFileById(fileId);
            if (file == null)
                return HttpNotFound();

            if (!User.Identity.IsAuthenticated && ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name) == null)
                return RedirectToAction("Index", "Home");

            if (type == FileActionsType.Delete)
            {
                var ipAdress = ConfigurationManager.AppSettings["Server"];
                
                try
                {
                    _fileService.DeleteFile(file, ipAdress, false);
                }
                catch (DataException)
                {
                    return RedirectToAction("FileDetails", new { fileId, page, messageType = ViewModelsMessageType.Error });
                }

                return RedirectToAction("UserFiles", new { page });
            }

            if (string.IsNullOrWhiteSpace(fileTags) || string.IsNullOrWhiteSpace(fileDescription))
                return RedirectToAction("FileDetails", new { fileId, page, messageType = ViewModelsMessageType.Default });
            try
            {
                _fileService.ChangeFile(file, fileTags, fileDescription);
            }
            catch (DataException)
            {
                return RedirectToAction("FileDetails", new { fileId, page, messageType = ViewModelsMessageType.Error });
            }

            return RedirectToAction("FileDetails", new { fileId, page, messageType = ViewModelsMessageType.Success });
        }

        [HttpPost]
        public ActionResult GetCommentsForFile(int fileId, int? page, ViewModelsMessageType? messageType)
        {
            var comments = _fileService.GetCommentsForFile(fileId);
            var commentsPerPages = comments == null
                ? null
                : comments.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

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

            var viewModel = new FileCommentsViewModel
            {
                FileId = fileId,
                Comments = commentsPerPages,
                PageInfo = pageInfo,
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.Default
                            ? "Error! The comment cannot be empty."
                            : messageType == ViewModelsMessageType.Error
                                ? "Error! The comment was not added."
                                : "Success! The comment was added."
                    }
                    : null
            };

            return PartialView("_FileCommentsPartial", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ViewModelsMessageType AddCommentToFile(int fileId, string newCommentText)
        {
            var file = _fileService.GetFileById(fileId);
            if (file == null)
                return ViewModelsMessageType.Error;

            if (string.IsNullOrWhiteSpace(newCommentText))
                return ViewModelsMessageType.Default;

            var user = User.Identity.IsAuthenticated
                ? ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name)
                : null;            
            try
            {
                _fileService.AddCommentToFile(newCommentText, file, user);
            }
            catch (DataException)
            {
                return ViewModelsMessageType.Error;
            }

            return ViewModelsMessageType.Success;
        }

        #endregion
    }
}