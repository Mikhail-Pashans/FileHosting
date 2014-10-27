using FileHosting.Domain.Enums;
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
using System.Web.Security;

namespace FileHosting.MVC.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {        
        private readonly HomeService _homeService;

        #region Constructor

        public HomeController()
        {            
            _homeService = new HomeService();
        }

        #endregion

        #region Actions

        [HttpGet]        
        public ActionResult Index(int? page)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");            

            var news = _homeService.GetActiveNews();

            var pageSize = int.Parse(ConfigurationManager.AppSettings.Get("NewsPageSize"));

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = news.Count
            };

            var pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            var newPerPages = news.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var fileSections = _homeService.GetFileSectionsDictianary();

            var viewModel = new HomeIndexViewModel
            {
                FileSections = fileSections,
                News = newPerPages,
                IsModerator = Roles.Provider.IsUserInRole(User.Identity.Name, "Moderator"),
                PageInfo = pageInfo
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public ActionResult AddNews(int? page, ViewModelsMessageType? messageType)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");
            
            if (((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name) == null)
                return RedirectToAction("Index", "Home");

            var pageNumber = (page ?? 1);

            return View(new AddNewsViewModel
            {
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.A
                            ? "Success! News was added."
                            : messageType == ViewModelsMessageType.B
                                ? "Error! News was not added."
                                : messageType == ViewModelsMessageType.C
                                    ? "Warning! News name cannot be empty."
                                    : messageType == ViewModelsMessageType.D
                                        ? "Warning! News text cannot be empty."
                                        : messageType == ViewModelsMessageType.E
                                        ? "Warning! News picture cannot be empty."
                                        : "Warning! News picture is not a picture."
                    }
                    : null,
                PageNumber = pageNumber
            });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddNews(int page, string newsName, string newsText, HttpPostedFileBase newsPicture)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");
            
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(newsName))
                return RedirectToAction("AddNews", new { page, messageType = ViewModelsMessageType.C });

            if (string.IsNullOrWhiteSpace(newsText))
                return RedirectToAction("AddNews", new { page, messageType = ViewModelsMessageType.D });

            if (newsPicture == null || newsPicture.ContentLength == 0)
                return RedirectToAction("AddNews", new { page, messageType = ViewModelsMessageType.E });

            var pictureName = Path.GetFileNameWithoutExtension(newsPicture.FileName);
            var pictureExtension = Path.GetExtension(newsPicture.FileName);
            if (pictureExtension != ".jpg" && pictureExtension != ".jpeg" && pictureExtension != ".png" && pictureExtension != ".gif")
                return RedirectToAction("AddNews", new { page, messageType = ViewModelsMessageType.F });

            var fullName = pictureName + Guid.NewGuid().ToString().Replace("-", "") + pictureExtension;

            var ipAdress = ConfigurationManager.AppSettings.Get("Server");

            var picturePath = string.Format("NewsPictures/{0}", fullName);

            var pathToSave = Path.Combine(ipAdress, picturePath);
            if (System.IO.File.Exists(pathToSave))
                System.IO.File.Delete(pathToSave);

            newsPicture.SaveAs(pathToSave);

            try
            {
                _homeService.AddNews(newsName, newsText, picturePath, user);
            }
            catch (DataException)
            {
                return RedirectToAction("AddNews", new { page, messageType = ViewModelsMessageType.B });
            }

            return RedirectToAction("AddNews", new { page, messageType = ViewModelsMessageType.A });
        }
        
        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public ActionResult EditNews(int newsId, int? page, ViewModelsMessageType? messageType)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");
            
            if (((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name) == null)
                return RedirectToAction("Index", "Home");            
            
            var newsModel = _homeService.GetModelForNews(newsId);
            if (newsModel == null)
                return HttpNotFound();

            var pageNumber = (page ?? 1);            

            return View(new EditNewsViewModel
            {
                NewsModel = newsModel,
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.A
                            ? "Success! News changes were saved."
                            : messageType == ViewModelsMessageType.B
                                ? "Error! News changes were not saved."
                                : messageType == ViewModelsMessageType.C
                                    ? "Warning! News name cannot be empty."
                                    : messageType == ViewModelsMessageType.D
                                        ? "Warning! News text cannot be empty."
                                        : messageType == ViewModelsMessageType.E
                                        ? "Warning! News picture cannot be empty."
                                        : "Warning! News picture is not a picture."
                    }
                    : null,
                PageNumber = pageNumber
            });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult EditNews(int newsId, int page, string newsName, string newsText, HttpPostedFileBase newNewsPicture)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");
            
            var user = ((MyMembershipProvider)Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            var news = _homeService.GetNewsById(newsId);
            if (news == null)
                return HttpNotFound();            

            if (string.IsNullOrWhiteSpace(newsName))
                return RedirectToAction("EditNews", new { newsId, page, messageType = ViewModelsMessageType.C });

            if (string.IsNullOrWhiteSpace(newsText))
                return RedirectToAction("EditNews", new { newsId, page, messageType = ViewModelsMessageType.D });

            if (newNewsPicture != null && newNewsPicture.ContentLength > 0)
            {
                var pictureName = Path.GetFileNameWithoutExtension(newNewsPicture.FileName);
                var pictureExtension = Path.GetExtension(newNewsPicture.FileName);
                if (pictureExtension != ".jpg" && pictureExtension != ".jpeg" && pictureExtension != ".png" && pictureExtension != ".gif")
                    return RedirectToAction("EditNews", new { newsId, page, messageType = ViewModelsMessageType.F });

                var fullName = pictureName + Guid.NewGuid().ToString().Replace("-", "") + pictureExtension;

                var ipAdress = ConfigurationManager.AppSettings.Get("Server");

                var oldPicturePath = Path.Combine(ipAdress, news.Picture);
                if (System.IO.File.Exists(oldPicturePath))
                    System.IO.File.Delete(oldPicturePath);

                var newPicturePath = string.Format("NewsPictures/{0}", fullName);

                var pathToSave = Path.Combine(ipAdress, newPicturePath);
                if (System.IO.File.Exists(pathToSave))
                    System.IO.File.Delete(pathToSave);

                newNewsPicture.SaveAs(pathToSave);
            }                       

            try
            {
                _homeService.ChangeNews(news, newsName, newsText, newPicturePath: null);
            }
            catch (DataException)
            {
                return RedirectToAction("EditNews", new { newsId, page, messageType = ViewModelsMessageType.B });
            }

            return RedirectToAction("EditNews", new { newsId, page, messageType = ViewModelsMessageType.A });
        }

        #endregion
    }
}