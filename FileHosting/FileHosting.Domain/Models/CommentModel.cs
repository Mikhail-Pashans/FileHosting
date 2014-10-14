using System;

namespace FileHosting.Domain.Models
{
    public class CommentModel
    {           
        public int CommentId { get; set; }
        public int Number { get; set; }
        public string Author { get; set; }

        private DateTime _publishDate;
        public DateTime PublishDate
        {
            get { return _publishDate.ToLocalTime(); }
            set { _publishDate = value; }
        }

        public string Text { get; set; }
    }
}
