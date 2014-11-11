using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web.Helpers;
using FileHosting.Database.Models;

namespace FileHosting.Database.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<UnitOfWork>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(UnitOfWork context)
        {
            context.Categories.AddOrUpdate(
                c => c.Id,
                new Category { Id = 1, Name = "Bookkeeping", Files = new List<File>() },
                new Category { Id = 2, Name = "Management", Files = new List<File>() },
                new Category { Id = 3, Name = "Support", Files = new List<File>() }
            );

            context.Users.AddOrUpdate(
                u => u.Id,
                new User { Id = 1, Name = "Admin", Email = "jolly.roger.1988@gmail.com", Password = Crypto.HashPassword("mike1988"), CreationDate = DateTime.UtcNow, DownloadAmountLimit = 0, DownloadSpeedLimit = 0, Comments = new List<Comment>(), Downloads = new List<Download>(), Files = new List<File>(), AllowedFiles = new List<File>(), SubscribedFiles = new List<File>(), News = new List<News>(), Roles = new List<Role>() },
                new User { Id = 2, Name = "User-1", Email = "poshlivsenahren@mail.ru", Password = Crypto.HashPassword("mike1988"), CreationDate = DateTime.UtcNow, DownloadAmountLimit = 0, DownloadSpeedLimit = 0, Comments = new List<Comment>(), Downloads = new List<Download>(), Files = new List<File>(), AllowedFiles = new List<File>(), SubscribedFiles = new List<File>(), News = new List<News>(), Roles = new List<Role>() },
                new User { Id = 3, Name = "User-2", Email = "mikhail.pashans@tut.by", Password = Crypto.HashPassword("mike1988"), CreationDate = DateTime.UtcNow, DownloadAmountLimit = 0, DownloadSpeedLimit = 0, Comments = new List<Comment>(), Downloads = new List<Download>(), Files = new List<File>(), AllowedFiles = new List<File>(), SubscribedFiles = new List<File>(), News = new List<News>(), Roles = new List<Role>() }
            );

            context.Roles.AddOrUpdate(
                r => r.Id,
                new Role { Id = 1, Name = "Administrator", Users = new List<User>{ context.Users.First(u => u.Name == "Admin") } },
                new Role { Id = 2, Name = "Moderator", Users = new List<User>() },
                new Role { Id = 3, Name = "RegisteredUser", Users = new List<User> { context.Users.First(u => u.Name == "User-1"), context.Users.First(u => u.Name == "User-2") } },
                new Role { Id = 4, Name = "BlockedUser", Users = new List<User>() }
            );            
        }
    }
}