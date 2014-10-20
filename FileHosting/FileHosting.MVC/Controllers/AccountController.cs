using FileHosting.Database;
using FileHosting.Domain.Enums;
using FileHosting.MVC.ViewModels;
using System.Web.Mvc;
using System.Web.Security;

namespace FileHosting.MVC.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _context;
        
        #region Constructor            

        public AccountController(IUnitOfWork context)
        {
            _context = context;
        }

        #endregion

        #region Actions

        [HttpGet]
        public ActionResult Login()
        {
            var viewModel = new LoginViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel viewModel, string returnUrl)
        {
            if (!ModelState.IsValid) return View(viewModel);

            if (Membership.ValidateUser(viewModel.Email, viewModel.Password))
            {
                FormsAuthentication.SetAuthCookie(viewModel.Email, viewModel.RememberMe);
                return RedirectToLocal(returnUrl);
            }

            viewModel.Message = new Message
            {
                MessageType = ViewModelsMessageType.B,
                MessageText = "The E-mail address or Password is incorrect!"
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult Register()
        {
            var viewModel = new RegisterViewModel();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            MembershipCreateStatus createStatus;
            Membership.CreateUser(viewModel.UserName, viewModel.Password, viewModel.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

            if (createStatus == MembershipCreateStatus.Success)
            {
                FormsAuthentication.SetAuthCookie(viewModel.Email, false);
                return RedirectToAction("Index", "Home");
            }

            viewModel.Message = new Message
            {
                MessageType = ViewModelsMessageType.B,
                MessageText = CreateStatusToString(createStatus)
            };

            return View(viewModel);
        }

        #endregion

        #region Private methods

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private static string CreateStatusToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "The same user name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return
                        "The same e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return
                        "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return
                        "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return
                        "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}