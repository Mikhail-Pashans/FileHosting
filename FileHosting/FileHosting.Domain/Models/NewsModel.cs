using System;

namespace FileHosting.Domain.Models
{
    public class NewsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Picture { get; set; }

        private DateTime _publishDate;

        public DateTime PublishDate
        {
            get { return _publishDate.ToLocalTime(); }
            set { _publishDate = value; }
        }        
    }
}