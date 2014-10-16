using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class UserDetailsViewModel
    {
        public UserModel UserModel { get; set; }
        public int PageNumber { get; set; }
    }
}