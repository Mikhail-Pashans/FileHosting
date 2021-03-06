﻿using FileHosting.Database.Models;
using System;
using System.Collections.Generic;

namespace FileHosting.Domain.Models
{
    public class UserModel
    {
        public int Id { get; set; }        
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }        
        public DateTime CreationDate { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
        
        public virtual ICollection<File> Files { get; set; }
        
        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<News> News { get; set; }

        public virtual ICollection<Download> Downloads { get; set; }

        public virtual ICollection<File> FilesWithPermissions { get; set; }
    }
}