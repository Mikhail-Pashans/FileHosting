using System.Globalization;
using FileHosting.Database;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FileHosting.Database.Models;

namespace FileHosting.Services
{
    public class HomeService
    {
        private readonly IUnitOfWork _context;

        public HomeService()
        {
            _context = DependencyResolver.Current.GetService<IUnitOfWork>();
        }

        public Dictionary<int, string> GetFileSectionDictianary()
        {
            return _context.SectionRepository
                .GetAll()
                .ToDictionary(s => s.Id, s=> s.Name);
        }

        public SelectList GetFileSectionSelectList()
        {
            return new SelectList(_context.SectionRepository
                .GetAll()
                .Select(s => new SelectListItem
                {
                    Text = s.Name,
                    Value = s.Id.ToString(CultureInfo.InvariantCulture)
                }),
                "Value",
                "Text");
        }

        public Section GetFileSectionById(int sectionId)
        {
            return _context.SectionRepository.GetById(sectionId);
        }
    }
}
