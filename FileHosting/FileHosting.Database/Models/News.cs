using System;

namespace FileHosting.Database.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Picture { get; set; }
        public DateTime PublishDate { get; set; }
        public bool IsActive { get; set; }

        public int AuthorId { get; set; }
        public virtual User Author { get; set; }
    }
}