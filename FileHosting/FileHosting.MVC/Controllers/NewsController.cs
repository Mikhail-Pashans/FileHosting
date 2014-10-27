using FileHosting.Database;
using FileHosting.Domain.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;

namespace FileHosting.MVC.Controllers
{
    public class NewsController : ApiController
    {        
        private readonly IUnitOfWork _context;

        #region Constructor

        public NewsController()
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        #endregion

        #region Actions

        //public IEnumerable<NewsModelJson> GetNewsInJson()
        //{
        //    var news = _context.NewsRepository.Find(n => n.IsActive)
        //        .Select(n => new NewsModelJson
        //        {
        //            Id = n.Id,
        //            Name = n.Name,
        //            Text = n.Text,
        //            Picture = n.Picture,
        //            PublishDateDate = n.PublishDate
        //        })
        //        .OrderByDescending(n => n.PublishDate)
        //        .ToList();

        //    return news;
        //}

        public string GetNewsInJson()
        {
            var news = _context.NewsRepository.Find(n => n.IsActive)
                .Select(n => new NewsModelJson
                {
                    Id = n.Id,
                    Name = n.Name,
                    Text = n.Text,
                    Picture = n.Picture,
                    PublishDate = n.PublishDate
                })
                .OrderByDescending(n => n.PublishDate)
                .ToList();

            return JsonConvert.SerializeObject(news);
        }

        #endregion                
    }
}
