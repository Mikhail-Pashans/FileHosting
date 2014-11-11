using System.Collections.Generic;
using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class UserFilesViewModel
    {        
        public List<FileModel> Files { get; set; }
        public string IdSortParm { get; set; }
        public string NameSortParm { get; set; }
        public string CategorySortParm { get; set; }
        public string SizeSortParm { get; set; }
        public string DateSortParm { get; set; }
        public string CurrentSort { get; set; }        
        public PageInfo PageInfo { get; set; }
    }
}