namespace FileHosting.MVC.ViewModels
{
    public class AddNewsViewModel : IViewModel
    {
        public int PageNumber { get; set; }
        public Message Message { get; set; }
    }
}