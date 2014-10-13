using FileHosting.Database;
using FileHosting.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FileHosting.Services
{
    public class NewsService
    {
        private readonly IUnitOfWork _context;

        public NewsService()
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        public List<News> GetActiveNews()
        {
            return _context.NewsRepository.Find(n => n.IsActive)
                .OrderByDescending(n => n.PublishDate)
                .ToList();
        }
    }
}
