using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class EditUserViewModel
    {
        public UserModel UserModel { get; set; }
        public string[] Roles { get; set; }
        public int PageNumber { get; set; }
    }
}