using System;
using System.Collections.Generic;
using FileHosting.Database.Models;

namespace FileHosting.Domain.Models
{
    public class FileModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Section Section { get; set; }
        public string Tags { get; set; }
        public string Description { get; set; }

        private DateTime _uploaDate;
        public DateTime UploadDate
        {
            get { return _uploaDate.ToLocalTime(); }
            set { _uploaDate = value; }
        }

        private long _size;
        public decimal Size
        {
            get { return decimal.Round((decimal) _size/1024, 2); }
            set { _size = (long) value; }
        }

        public string Path { get; set; }
        public User Owner { get; set; }
        public List<Download> Downloads { get; set; }
        public bool IsAllowedAnonymousBrowsing { get; set; }
        public bool IsAllowedAnonymousAction { get; set; }
        public List<User> AllowedUsers { get; set; }
    }
}