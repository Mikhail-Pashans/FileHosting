using System.Collections.Generic;
using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class FilesIndexViewModel
    {
        public Dictionary<int, string> FileSectionDictionary { get; set; }
        public List<FileModel> Files { get; set; }
        public string IdSortParm { get; set; }
        public string NameSortParm { get; set; }
        public string SectionSortParm { get; set; }
        public string SizeSortParm { get; set; }
        public string DateSortParm { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public bool IsAuthenticated { get; set; }
        public int SectionNumber { get; set; }
        public PageInfo PageInfo { get; set; }
    }
}