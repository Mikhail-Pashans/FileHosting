using FileHosting.Domain.Enums;
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
        public ActionResult EditUser(int userId, int? page, ViewModelsMessageType? messageType)
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
                Message = messageType.HasValue
                    ? new Message
                    {
                        MessageType = messageType.Value,
                        MessageText = messageType == ViewModelsMessageType.A
                            ? "Success! User changes were saved."
                            : messageType == ViewModelsMessageType.B
                                ? "Error! User changes were not saved."
                                : messageType == ViewModelsMessageType.C
                                    ? "Warning! User roles cannot be empty."
                                    : messageType == ViewModelsMessageType.D
                                        ? "Warning! Download amount limit must be a number."
                                        : "Warning! Download speed limit must be a number."
                    }
                    : null
            };

            return View(viewModel);           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(int userId, string userEmail, int page, string[] userRoles, string newUserPassword, string downloadAmountLimit, string downloadSpeedLimit)
        {
            if (userRoles == null)
                return RedirectToAction("EditUser", new { userId, page, messageType = ViewModelsMessageType.C });

            try
            {
                Roles.Provider.AddUsersToRoles(new[] { userEmail }, userRoles);
            }
            catch (DataException)
            {
                return RedirectToAction("EditUser", new { userId, page, messageType = ViewModelsMessageType.B });
            }

            decimal amountLimit = 0, speedLimit = 0;

            if (!string.IsNullOrWhiteSpace(downloadAmountLimit))
            {
                if (!decimal.TryParse(downloadAmountLimit, out amountLimit))
                {
                    return RedirectToAction("EditUser", new { userId, page, messageType = ViewModelsMessageType.D });
                }
            }

            if (!string.IsNullOrWhiteSpace(downloadSpeedLimit))
            {                
                if (!decimal.TryParse(downloadSpeedLimit, out speedLimit))
                {
                    return RedirectToAction("EditUser", new { userId, page, messageType = ViewModelsMessageType.E });
                }
            }

            bool result;
            
            if (!string.IsNullOrWhiteSpace(newUserPassword))
            {
                result = ((MyMembershipProvider)Membership.Provider).ChangeUserPassword(userEmail, newUserPassword);
                if (!result)
                    return RedirectToAction("EditUser", new { userId, page, messageType = ViewModelsMessageType.B }); 
            }

            result = _homeService.SaveUserChanges(userId, amountLimit, speedLimit);

            return RedirectToAction("EditUser", new { userId, page, messageType = result ? ViewModelsMessageType.A : ViewModelsMessageType.B });
        }

        #endregion        
    }
}