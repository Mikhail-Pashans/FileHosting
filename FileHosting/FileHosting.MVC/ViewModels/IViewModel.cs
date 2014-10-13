using FileHosting.Domain.Enums;
using System;

namespace FileHosting.MVC.ViewModels
{
    public interface IViewModel
    {
        Message Message { get; set; }
    }

    public class Message
    {
        public string MessageText { get; set; }

        public ViewModelsMessageType MessageType { get; set; }
    }

    public class PageInfo
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / PageSize); }
        }
    }
}