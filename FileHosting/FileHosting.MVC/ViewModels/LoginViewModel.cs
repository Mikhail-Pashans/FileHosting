using System.ComponentModel.DataAnnotations;

namespace FileHosting.MVC.ViewModels
{
    public class LoginViewModel : IViewModel
    {
        [Required(ErrorMessage = "The \"E-mail address\" field is required!")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail address *")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The \"Password\" field is required!")]
        [DataType(DataType.Password)]
        [Display(Name = "Password *")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public Message Message { get; set; }
    }
}