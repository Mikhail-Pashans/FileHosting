using FileHosting.Database.Models;
using System;

namespace FileHosting.Domain.Models
{
    public class DownloadModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int FileId { get; set; }
        public virtual File File { get; set; }

        public int? UserId { get; set; }
        public virtual User User { get; set; }
    }
}