using FileHosting.Database.Models;
using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<News> News { get; set; }
        public PageInfo PageInfo { get; set; }
        public Dictionary<int, string> FileSections { get; set; }
    }
}