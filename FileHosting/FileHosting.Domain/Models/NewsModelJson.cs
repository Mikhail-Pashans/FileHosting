using System;

namespace FileHosting.Domain.Models
{
    public class NewsModelJson
    {        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Picture { get; set; }

        public DateTime PublishDate { get; set; }

        public string PublishDateString
        {
            get { return PublishDate.ToLocalTime().ToString("G"); }
        }
    }
}