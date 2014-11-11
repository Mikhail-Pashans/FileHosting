using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FileHosting.Database.Models;
using FileHosting.Domain.Enums;
using FileHosting.Domain.Models;
using FileHosting.MVC.Providers;
using FileHosting.MVC.ViewModels;
using FileHosting.Services;

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

            List<NewsModel> news = _homeService.GetActiveNews();

            int pageSize = int.Parse(ConfigurationManager.AppSettings.Get("NewsPageSize"));

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = news.Count
            };

            int pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            List<NewsModel> newPerPages = news.Skip((pageNumber - 1)*pageSize).Take(pageSize).ToList();

            var viewModel = new HomeIndexViewModel
            {
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

            if (((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name) == null)
                return RedirectToAction("Index", "Home");

            int pageNumber = (page ?? 1);

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

            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrWhiteSpace(newsName))
                return RedirectToAction("AddNews", new {page, messageType = ViewModelsMessageType.C});

            if (string.IsNullOrWhiteSpace(newsText))
                return RedirectToAction("AddNews", new {page, messageType = ViewModelsMessageType.D});

            if (newsPicture == null || newsPicture.ContentLength == 0)
                return RedirectToAction("AddNews", new {page, messageType = ViewModelsMessageType.E});

            string pictureName = Path.GetFileNameWithoutExtension(newsPicture.FileName);
            string pictureExtension = Path.GetExtension(newsPicture.FileName);
            if (pictureExtension != ".jpg" && pictureExtension != ".jpeg" && pictureExtension != ".png" &&
                pictureExtension != ".gif")
                return RedirectToAction("AddNews", new {page, messageType = ViewModelsMessageType.F});

            string fullName = pictureName + Guid.NewGuid().ToString().Replace("-", "") + pictureExtension;

            string ipAdress = ConfigurationManager.AppSettings.Get("Server");

            string picturePath = string.Format("NewsPictures/{0}", fullName);

            string pathToSave = Path.Combine(ipAdress, picturePath);
            if (System.IO.File.Exists(pathToSave))
                System.IO.File.Delete(pathToSave);

            newsPicture.SaveAs(pathToSave);

            try
            {
                _homeService.AddNews(newsName, newsText, picturePath, user);
            }
            catch (DataException)
            {
                return RedirectToAction("AddNews", new {page, messageType = ViewModelsMessageType.B});
            }

            return RedirectToAction("AddNews", new {page, messageType = ViewModelsMessageType.A});
        }

        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public ActionResult EditNews(int newsId, int? page, ViewModelsMessageType? messageType)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            if (((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name) == null)
                return RedirectToAction("Index", "Home");

            NewsModel newsModel = _homeService.GetModelForNews(newsId);
            if (newsModel == null)
                return HttpNotFound();

            int pageNumber = (page ?? 1);

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
        public ActionResult EditNews(int newsId, int page, string newsName, string newsText,
            HttpPostedFileBase newNewsPicture)
        {
            if (Roles.Provider.IsUserInRole(User.Identity.Name, "BlockedUser"))
                return View("_UnauthorizedAccessAttemp");

            User user = ((MyMembershipProvider) Membership.Provider).GetUserByEmail(User.Identity.Name);
            if (user == null)
                return RedirectToAction("Index", "Home");

            News news = _homeService.GetNewsById(newsId);
            if (news == null)
                return HttpNotFound();

            if (string.IsNullOrWhiteSpace(newsName))
                return RedirectToAction("EditNews", new {newsId, page, messageType = ViewModelsMessageType.C});

            if (string.IsNullOrWhiteSpace(newsText))
                return RedirectToAction("EditNews", new {newsId, page, messageType = ViewModelsMessageType.D});

            if (newNewsPicture != null && newNewsPicture.ContentLength > 0)
            {
                string pictureName = Path.GetFileNameWithoutExtension(newNewsPicture.FileName);
                string pictureExtension = Path.GetExtension(newNewsPicture.FileName);
                if (pictureExtension != ".jpg" && pictureExtension != ".jpeg" && pictureExtension != ".png" &&
                    pictureExtension != ".gif")
                    return RedirectToAction("EditNews", new {newsId, page, messageType = ViewModelsMessageType.F});

                string fullName = pictureName + Guid.NewGuid().ToString().Replace("-", "") + pictureExtension;

                string ipAdress = ConfigurationManager.AppSettings.Get("Server");

                string oldPicturePath = Path.Combine(ipAdress, news.Picture);
                if (System.IO.File.Exists(oldPicturePath))
                    System.IO.File.Delete(oldPicturePath);

                string newPicturePath = string.Format("NewsPictures/{0}", fullName);

                string pathToSave = Path.Combine(ipAdress, newPicturePath);
                if (System.IO.File.Exists(pathToSave))
                    System.IO.File.Delete(pathToSave);

                newNewsPicture.SaveAs(pathToSave);
            }

            try
            {
                _homeService.ChangeNews(news, newsName, newsText, null);
            }
            catch (DataException)
            {
                return RedirectToAction("EditNews", new {newsId, page, messageType = ViewModelsMessageType.B});
            }

            return RedirectToAction("EditNews", new {newsId, page, messageType = ViewModelsMessageType.A});
        }        

        #endregion
    }
}