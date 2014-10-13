﻿using FileHosting.Database.Models;
using System.Collections.Generic;

namespace FileHosting.Domain.Models
{
    public class SectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<File> Files { get; set; }
    }
}