using System;

namespace FileHosting.Domain.Models
{
    public class CommentModel
    {           
        public int CommentId { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public string Text { get; set; }
    }
}
