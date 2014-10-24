using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class EditUserViewModel : IViewModel
    {
        public UserModel UserModel { get; set; }
        public string[] Roles { get; set; }
        public Message Message { get; set; }
        public int PageNumber { get; set; }
    }
}