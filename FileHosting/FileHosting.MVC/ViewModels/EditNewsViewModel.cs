using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class EditNewsViewModel : IViewModel
    {
        public NewsModel NewsModel { get; set; }
        public Message Message { get; set; }
        public int PageNumber { get; set; }
    }
}