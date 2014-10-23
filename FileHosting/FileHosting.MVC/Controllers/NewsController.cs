using FileHosting.Database;
using FileHosting.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace FileHosting.MVC.Controllers
{
    public class NewsController : ApiController
    {
        //private readonly HomeService _homeService;
        private readonly IUnitOfWork _context;

        public NewsController()
        {
            //_homeService = new HomeService();
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        public IEnumerable<NewsModel> GetNewsInJson()
        {
            var news = _context.NewsRepository.Find(n => n.IsActive)
                .Select(n => new NewsModel
                {
                    Id = n.Id,
                    Name = n.Name,
                    Text = n.Text,
                    Picture = n.Picture,
                    PublishDate = n.PublishDate
                })
                .OrderByDescending(n => n.PublishDate)
                .ToList();

            return news;            
        }
    }
}
