using FileHosting.MVC.ViewModels;
using FileHosting.Services;
using System.Linq;
using System.Web.Mvc;

namespace FileHosting.MVC.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly NewsService _newsService;
        private readonly HomeService _homeService;

        #region Constructor

        public HomeController()
        {
            _newsService = new NewsService();
            _homeService = new HomeService();
        }

        #endregion

        #region Actions

        [HttpGet]
        public ActionResult Index(int? page)
        {
            var news = _newsService.GetActiveNews();

            const int pageSize = 10;

            var pageInfo = new PageInfo
            {
                PageSize = pageSize,
                TotalItems = news.Count
            };

            var pageNumber = (page ?? 1);
            if (pageNumber > pageInfo.TotalPages)
                pageNumber = pageInfo.TotalPages;
            pageInfo.PageNumber = pageNumber;

            var newPerPages = news.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var fileSections = _homeService.GetFileSectionDictianary();

            var viewModel = new HomeIndexViewModel
            {
                News = newPerPages,
                PageInfo = pageInfo,
                FileSections = fileSections
            };

            return View(viewModel);
        }

        #endregion
    }
}