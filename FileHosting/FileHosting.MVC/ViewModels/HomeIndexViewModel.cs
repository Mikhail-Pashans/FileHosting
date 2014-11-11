using System.Collections.Generic;
using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<NewsModel> News { get; set; }
        public bool IsModerator { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}