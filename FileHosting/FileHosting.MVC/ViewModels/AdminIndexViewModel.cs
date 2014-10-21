using FileHosting.Domain.Models;
using System.Collections.Generic;

namespace FileHosting.MVC.ViewModels
{
    public class AdminIndexViewModel : IViewModel
    {
        public List<UserModel> Users { get; set; }
        public decimal TotalDownloadAmountLimit { get; set; }
        public decimal TotalDownloadSpeedLimit { get; set; }
        public PageInfo PageInfo { get; set; }
        public Message Message { get; set; }
    }
}