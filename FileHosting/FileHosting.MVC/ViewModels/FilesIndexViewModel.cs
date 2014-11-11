using System.Collections.Generic;
using FileHosting.Domain.Enums;
using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class FilesIndexViewModel
    {
        public List<FileModel> Files { get; set; }
        public SearchFilesType SearchType { get; set; }
        public string SearchParam { get; set; }
        public string SortOrder { get; set; }        
        public string IdSortParm { get; set; }
        public string NameSortParm { get; set; }
        public string CategorySortParm { get; set; }
        public string SizeSortParm { get; set; }
        public string DateSortParm { get; set; }        
        public bool IsAuthenticated { get; set; }        
        public PageInfo PageInfo { get; set; }
    }
}