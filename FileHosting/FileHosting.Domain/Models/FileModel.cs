using FileHosting.Database.Models;
using System;
using System.Collections.Generic;

namespace FileHosting.Domain.Models
{
    public class FileModel
    {        
        public int Id { get; set; }
        public string Name { get; set; }
        public Section Section { get; set; }
        public List<Tag> Tags { get; set; }
        public string Description { get; set; }        
        
        private DateTime _uploaDate;
        public DateTime UploadDate
        {
            get { return _uploaDate.ToLocalTime(); }
            set { _uploaDate = value; }
        }

        public decimal Size { get; set; }        
        public string Path { get; set; }
        public User Owner { get; set; }
        public bool IsAllowedAnonymousBrowsing { get; set; }
        public bool IsAllowedAnonymousComments { get; set; }
    }    
}
