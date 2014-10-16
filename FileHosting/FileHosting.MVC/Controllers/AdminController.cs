using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using FileHosting.MVC.Providers;
using FileHosting.MVC.ViewModels;
using FileHosting.Services;

namespace FileHosting.MVC.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {        
        private readonly FileService _fileService;
        private readonly HomeService _homeService;

        #region Constructor

        public AdminController()
        {
            _fileService = new FileService();
            _homeService = new HomeService();
        }

        #endregion

        #region Actions

        [HttpGet]
        public ActionResult Index(int? page)
        {
            var users = _homeService.GetAllUsersList();
            
            const int pageSize = 10;

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = users.Count
            };

            var pageNumber = (page ?? 1);
            if (pageNumber < 1)
                pageNumber = 1;
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            var usersPerPages = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var viewModel = new AdminIndexViewModel
            {
                Users = usersPerPages,
                PageInfo = pageInfo,                
            };

            return View(viewModel);
        }

        public ActionResult UserDetails(int userId, int? page)
        {
            var user = _homeService.GetUserById(userId);
            if (user == null)
                return HttpNotFound();            

            var pageNumber = (page ?? 1);

            var viewModel = new UserDetailsViewModel
            {
                UserModel = user,                
                PageNumber = pageNumber,                
            };

            return View(viewModel);           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeUserPassword(int userId, string newUserPassword, int page)
        {
            var result = ((MyMembershipProvider)Membership.Provider).ChangeUserPassword(userId, newUserPassword);

            return result ? RedirectToAction("UserDetails", new { userId, page }) : RedirectToAction("Index");
        }

        public ActionResult BlockUser(int userId)
        {
            return new EmptyResult();
        }

        #endregion        
    }
}