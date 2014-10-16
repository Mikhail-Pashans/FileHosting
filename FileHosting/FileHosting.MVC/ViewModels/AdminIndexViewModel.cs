using FileHosting.Database.Models;
using FileHosting.Domain.Models;
using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class AdminIndexViewModel
    {
        public List<UserModel> Users { get; set; }
        public PageInfo PageInfo { get; set; }        
    }
}