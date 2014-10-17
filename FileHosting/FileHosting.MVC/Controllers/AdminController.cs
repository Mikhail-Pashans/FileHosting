using FileHosting.MVC.Providers;
using FileHosting.MVC.ViewModels;
using FileHosting.Services;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

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

        [HttpGet]
        public ActionResult EditUser(int userId, int? page)
        {
            var user = _homeService.GetUserById(userId);
            if (user == null)
                return HttpNotFound();            

            var pageNumber = (page ?? 1);

            var viewModel = new EditUserViewModel
            {
                UserModel = user,
                Roles = Roles.Provider.GetAllRoles(),
                PageNumber = pageNumber,
            };

            return View(viewModel);           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(int userId, string userEmail, int page, string[] userRoles, string newUserPassword = null)
        {
            if (userRoles == null)
                return RedirectToAction("Index", new { page });
            
            try
            {
                Roles.Provider.AddUsersToRoles(new[] { userEmail }, userRoles);
            }
            catch (DataException)
            {                
                return RedirectToAction("Index", new { page });
            }

            var result = true;
            
            if (!string.IsNullOrWhiteSpace(newUserPassword))
            {
                result = ((MyMembershipProvider)Membership.Provider).ChangeUserPassword(userEmail, newUserPassword);
            }            

            return result ? RedirectToAction("EditUser", new { userId, page }) : RedirectToAction("Index", new { page });
        }

        public ActionResult BlockUser(int userId)
        {
            return new EmptyResult();
        }

        #endregion        
    }
}