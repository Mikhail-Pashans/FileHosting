using FileHosting.Domain.Models;
using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class HomeIndexViewModel
    {
        public Dictionary<int, string> FileSections { get; set; }
        public List<NewsModel> News { get; set; }
        public bool IsModerator { get; set; }
        public PageInfo PageInfo { get; set; }        
    }
}