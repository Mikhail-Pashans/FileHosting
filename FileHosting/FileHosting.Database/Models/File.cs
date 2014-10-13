using System;
using System.Collections.Generic;

namespace FileHosting.Database.Models
{
    public class File
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public decimal Size { get; set; }
        public string Path { get; set; }
        public bool IsAllowedAnonymousBrowsing { get; set; }
        public bool IsAllowedAnonymousComments { get; set; }

        public int SectionId { get; set; }
        public virtual Section Section { get; set; }
        
        public int OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public virtual ICollection<User> AllowedUsers { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Download> Downloads { get; set; }
        
        public virtual ICollection<Tag> Tags { get; set; }                        
    }
}