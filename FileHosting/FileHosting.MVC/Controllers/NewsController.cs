using FileHosting.Database;
using System.Web.Mvc;

namespace FileHosting.MVC.Controllers
{
    [Authorize (Roles="Administrator, Moderator")]
    public class NewsController : Controller
    {
        private readonly IUnitOfWork _context;

        #region Constructor

        public NewsController(IUnitOfWork context)
        {
            _context = context;
        }

        #endregion

        #region Actions

        public ActionResult Index()
        {
            return View();
        }

        #endregion
    }
}