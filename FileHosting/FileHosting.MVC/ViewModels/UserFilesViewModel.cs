using FileHosting.Domain.Models;
using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class UserFilesViewModel
    {
        public List<FileModel> Files { get; set; }
        public PageInfo PageInfo { get; set; }
        public Dictionary<int, string> FileSectionsDictionary { get; set; }        
    }
}