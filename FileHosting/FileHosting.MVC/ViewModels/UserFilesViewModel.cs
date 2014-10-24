using FileHosting.Domain.Models;
using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class UserFilesViewModel
    {
        public Dictionary<int, string> FileSectionsDictionary { get; set; }
        public List<FileModel> Files { get; set; }
        public string IdSortParm { get; set; }
        public string NameSortParm { get; set; }
        public string SectionSortParm { get; set; }
        public string SizeSortParm { get; set; }
        public string DateSortParm { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public PageInfo PageInfo { get; set; }                
    }
}