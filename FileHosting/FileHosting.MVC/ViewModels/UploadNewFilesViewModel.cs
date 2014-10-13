using System.Web.Mvc;

namespace FileHosting.MVC.ViewModels
{
    public class UploadNewFilesViewModel
    {
        public SelectList FileSectionSelectList { get; set; }
        public int PageNumber { get; set; }
    }
}