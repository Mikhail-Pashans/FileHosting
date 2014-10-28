using System.Collections.Generic;

namespace FileHosting.Database.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<File> Files { get; set; }
    }
}