﻿using FileHosting.Domain.Models;

namespace FileHosting.MVC.ViewModels
{
    public class FileDetailsViewModel : IViewModel
    {
        public FileModel FileModel { get; set; }
        public int Section { get; set; }
        public int PageNumber { get; set; }
        public Message Message { get; set; }
    }
}