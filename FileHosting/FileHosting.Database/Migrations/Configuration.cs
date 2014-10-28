using System.Data.Entity.Migrations;

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
            //var sections = new List<Section>
            //{
            //    new Section { Name = "Bookkeeping", Files = new List<File>() },
            //    new Section { Name = "Management", Files = new List<File>() },
            //    new Section { Name = "Support", Files = new List<File>() }
            //};

            //foreach (var section in sections)
            //{
            //    context.SectionRepository.Add(section);
            //}

            //var admin = new User
            //{
            //    Name = "Admin",
            //    Email = "jolly.roger.1988@gmail.com",
            //    Password = Crypto.HashPassword("mike1988"),
            //    CreationDate = DateTime.UtcNow,
            //    DownloadAmountLimit = 0,
            //    DownloadSpeedLimit = 0,
            //    Comments = new List<Comment>(),
            //    Downloads = new List<Download>(),
            //    Files = new List<File>(),
            //    FilesWithPermission = new List<File>(),
            //    FilesWithSubscription = new List<File>(),
            //    News = new List<News>(),
            //    Roles = new List<Role>()
            //};

            //var user1 = new User
            //{
            //    Name = "User-1",
            //    Email = "poshlivsenahren@mail.ru",
            //    Password = Crypto.HashPassword("mike1988"),
            //    CreationDate = DateTime.UtcNow,
            //    DownloadAmountLimit = 0,
            //    DownloadSpeedLimit = 0,
            //    Comments = new List<Comment>(),
            //    Downloads = new List<Download>(),
            //    Files = new List<File>(),
            //    FilesWithPermission = new List<File>(),
            //    FilesWithSubscription = new List<File>(),
            //    News = new List<News>(),
            //    Roles = new List<Role>()
            //};

            //var user2 = new User
            //{
            //    Name = "User-2",
            //    Email = "mikhail.pashans@tut.by",
            //    Password = Crypto.HashPassword("mike1988"),
            //    CreationDate = DateTime.UtcNow,
            //    DownloadAmountLimit = 0,
            //    DownloadSpeedLimit = 0,
            //    Comments = new List<Comment>(),
            //    Downloads = new List<Download>(),
            //    Files = new List<File>(),
            //    FilesWithPermission = new List<File>(),
            //    FilesWithSubscription = new List<File>(),
            //    News = new List<News>(),
            //    Roles = new List<Role>()
            //};

            //var users = new List<User> { admin, user1, user2 };

            //foreach (var user in users)
            //{
            //    context.UserRepository.Add(user);
            //}

            //var roles = new List<Role>
            //{
            //    new Role { Id = 1, Name = "Administrator", Users = new List<User>{ admin } },
            //    new Role { Id = 2, Name = "Moderator", Users = new List<User>() },
            //    new Role { Id = 3, Name = "RegisteredUser", Users = new List<User>{ user1, user2 } },
            //    new Role { Id = 4, Name = "BlockedUser", Users = new List<User>() }
            //};

            //foreach (var role in roles)
            //{
            //    context.RoleRepository.Add(role);
            //}

            //context.Commit();
        }
    }
}