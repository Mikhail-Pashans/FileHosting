﻿using System;
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
        public long Size { get; set; }
        public string Path { get; set; }
        public bool IsAllowedAnonymousBrowsing { get; set; }
        public bool IsAllowedAnonymousAction { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public int OwnerId { get; set; }
        public virtual User Owner { get; set; }

        public virtual ICollection<User> AllowedUsers { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Download> Downloads { get; set; }

        public virtual ICollection<User> SubscribedUsers { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}