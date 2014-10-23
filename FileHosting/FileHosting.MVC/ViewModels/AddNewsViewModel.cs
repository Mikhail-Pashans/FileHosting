namespace FileHosting.MVC.ViewModels
{
    public class AddNewsViewModel : IViewModel
    {        
        public Message Message { get; set; }
        public int PageNumber { get; set; }
    }
}