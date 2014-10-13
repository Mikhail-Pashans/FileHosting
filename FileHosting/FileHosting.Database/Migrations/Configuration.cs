using FileHosting.Database.Models;
using System.Collections.Generic;
using System.Web.Helpers;

namespace FileHosting.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<FileHosting.Database.UnitOfWork>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FileHosting.Database.UnitOfWork context)
        {
            //var user = new User
            //{
            //    Name = "admin",
            //    Email = "jolly.roger.1988@gmail.com",
            //    Password = Crypto.HashPassword("mike1988"),
            //    CreationDate = DateTime.UtcNow,
            //    Comments = new List<Comment>(),
            //    Downloads = new List<Download>(),
            //    Files = new List<File>(),
            //    FilesWithPermissions = new List<File>(),
            //    News = new List<News>(),
            //    Roles = new List<Role>(),
            //};
            //context.UserRepository.Add(user);

            //var roles = new List<Role>
            //{
            //    new Role { Name = "Administrator", Users = new List<User>() },
            //    new Role { Name = "Moderator", Users = new List<User>() },
            //    new Role { Name = "RegisteredUser", Users = new List<User>() }
            //};

            //foreach (var role in roles)
            //{
            //    context.RoleRepository.Add(role);
            //}

            //var sections = new List<Section>
            //{
            //    new Section { Name = "Bookkeeping", Files = new List<File>() },
            //    new Section { Name = "Management", Files = new List<File>() },
            //    new Section { Name = "Support", Files = new List<File>() },
            //};

            //foreach (var section in sections)
            //{
            //    context.SectionRepository.Add(section);
            //}

            //context.Commit();

            //var userRole = context.RoleRepository.GetById(1);
            //if (userRole != null) user.Roles.Add(userRole);

            //context.Commit();
        }
    }
}