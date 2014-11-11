using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class UploadNewFilesViewModel
    {
        public Dictionary<int, string> CategoriesDictionary { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public int PageNumber { get; set; }
    }
}