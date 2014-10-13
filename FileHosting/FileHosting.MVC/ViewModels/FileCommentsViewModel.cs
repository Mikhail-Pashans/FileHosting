using FileHosting.Domain.Models;
using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class FileCommentsViewModel : IViewModel
    {
        public int FileId { get; set; }
        public List<CommentModel> Comments { get; set; }
        public PageInfo PageInfo { get; set; }
        public Message Message { get; set; }
    }
}