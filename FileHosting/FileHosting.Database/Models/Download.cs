using System;

namespace FileHosting.Database.Models
{
    public class Download
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int FileId { get; set; }
        public virtual File File { get; set; }

        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}