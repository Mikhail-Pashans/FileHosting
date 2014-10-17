using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class UploadNewFilesViewModel
    {
        public Dictionary<int, string> FileSectionsDictionary { get; set; }
        public int PageNumber { get; set; }
    }
}