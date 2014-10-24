using FileHosting.Domain.Models;
using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class FilesIndexViewModel
    {
        public Dictionary<int, string> FileSectionDictionary { get; set; }
        public List<FileModel> Files { get; set; }
        public bool IsAuthenticated { get; set; }
        public int SectionNumber { get; set; }
        public PageInfo PageInfo { get; set; }        
    }
}