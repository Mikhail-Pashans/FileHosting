using SuperCaptcha.Mvc.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace FileHosting.MVC.ViewModels
{
    public class RegisterViewModel : IViewModel
    {
        [Required(ErrorMessage = "The \"User name\" field is required!")]
        [StringLength(50, ErrorMessage = "User name length must be between 3 and 50 characters!", MinimumLength = 3)]
        [Display(Name = "User name *")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The \"E-mail address\" field is required!")]
        [StringLength(200, ErrorMessage = "E-mail address length must be between 6 and 200 characters!", MinimumLength = 6)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?",
            ErrorMessage = "E-mail address is not valid!")]
        [Display(Name = "E-mail address *")]
        public string Email { get; set; }        

        [Required(ErrorMessage = "The \"Password\" field is required!")]
        [StringLength(100, ErrorMessage = "Password length must be between 6 and 100 characters!", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password *")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The \"Confirm password\" field is required!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password *")]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "The \"Captcha\" field is required!")]
        [StringLength(5, ErrorMessage = "Captcha length must be 5 characters!", MinimumLength = 5)]
        [Display(Name = "Captcha *")]
        [VerifyCaptcha]
        public string CaptchaText { get; set; }

        public Message Message { get; set; }
    }
}