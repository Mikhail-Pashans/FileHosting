﻿using System.Collections.Generic;
using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class FileDetailsViewModel : IViewModel
    {
        public FileModel FileModel { get; set; }
        public List<UserModel> Users { get; set; }
        public int SectionNumber { get; set; }
        public int PageNumber { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsSubscribed { get; set; }
        public bool IsOwner { get; set; }
        public string CurrentSort { get; set; }
        public string CurrentFilter { get; set; }
        public Message Message { get; set; }
    }
}