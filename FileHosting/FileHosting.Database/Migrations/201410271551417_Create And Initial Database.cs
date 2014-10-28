using System.Data.Entity.Migrations;

namespace FileHosting.Database.Migrations
{
    public partial class CreateAndInitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                {
                    Id = c.Int(false, true),
                    Text = c.String(false),
                    PublishDate = c.DateTime(false),
                    IsActive = c.Boolean(false),
                    FileId = c.Int(false),
                    AuthorId = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .ForeignKey("dbo.Files", t => t.FileId, true)
                .Index(t => t.FileId)
                .Index(t => t.AuthorId);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id = c.Int(false, true),
                    Name = c.String(false, 50),
                    Email = c.String(false, 50),
                    Password = c.String(false, 70),
                    CreationDate = c.DateTime(false),
                    DownloadAmountLimit = c.Decimal(false, 10, 2),
                    DownloadSpeedLimit = c.Decimal(false, 10, 2),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Downloads",
                c => new
                {
                    Id = c.Int(false, true),
                    Date = c.DateTime(false),
                    FileId = c.Int(false),
                    UserId = c.Int(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.FileId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Files",
                c => new
                {
                    Id = c.Int(false, true),
                    Name = c.String(false, 300),
                    FullName = c.String(false, 300),
                    Description = c.String(),
                    UploadDate = c.DateTime(false),
                    Size = c.Long(false),
                    Path = c.String(false, 300),
                    IsAllowedAnonymousBrowsing = c.Boolean(false),
                    IsAllowedAnonymousAction = c.Boolean(false),
                    SectionId = c.Int(false),
                    OwnerId = c.Int(false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.OwnerId)
                .ForeignKey("dbo.Sections", t => t.SectionId, true)
                .Index(t => t.SectionId)
                .Index(t => t.OwnerId);

            CreateTable(
                "dbo.Sections",
                c => new
                {
                    Id = c.Int(false, true),
                    Name = c.String(false, 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Tags",
                c => new
                {
                    Id = c.Int(false, true),
                    Name = c.String(false, 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.News",
                c => new
                {
                    Id = c.Int(false, true),
                    Name = c.String(false),
                    Text = c.String(false),
                    Picture = c.String(false, 300),
                    PublishDate = c.DateTime(false),
                    IsActive = c.Boolean(false),
                    AuthorId = c.Int(false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .Index(t => t.AuthorId);

            CreateTable(
                "dbo.Roles",
                c => new
                {
                    Id = c.Int(false, true),
                    Name = c.String(false, 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Files&PermittedUsers",
                c => new
                {
                    FileId = c.Int(false),
                    UserId = c.Int(false),
                })
                .PrimaryKey(t => new {t.FileId, t.UserId})
                .ForeignKey("dbo.Files", t => t.FileId, true)
                .ForeignKey("dbo.Users", t => t.UserId, true)
                .Index(t => t.FileId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Files&SubscribedUsers",
                c => new
                {
                    FileId = c.Int(false),
                    UserId = c.Int(false),
                })
                .PrimaryKey(t => new {t.FileId, t.UserId})
                .ForeignKey("dbo.Files", t => t.FileId, true)
                .ForeignKey("dbo.Users", t => t.UserId, true)
                .Index(t => t.FileId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.Files&Tags",
                c => new
                {
                    FileId = c.Int(false),
                    TagId = c.Int(false),
                })
                .PrimaryKey(t => new {t.FileId, t.TagId})
                .ForeignKey("dbo.Files", t => t.FileId, true)
                .ForeignKey("dbo.Tags", t => t.TagId, true)
                .Index(t => t.FileId)
                .Index(t => t.TagId);

            CreateTable(
                "dbo.Users&Roles",
                c => new
                {
                    UserId = c.Int(false),
                    RoleId = c.Int(false),
                })
                .PrimaryKey(t => new {t.UserId, t.RoleId})
                .ForeignKey("dbo.Users", t => t.UserId, true)
                .ForeignKey("dbo.Roles", t => t.RoleId, true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Comments", "FileId", "dbo.Files");
            DropForeignKey("dbo.Comments", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.Users&Roles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Users&Roles", "UserId", "dbo.Users");
            DropForeignKey("dbo.News", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.Downloads", "UserId", "dbo.Users");
            DropForeignKey("dbo.Downloads", "FileId", "dbo.Files");
            DropForeignKey("dbo.Files&Tags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.Files&Tags", "FileId", "dbo.Files");
            DropForeignKey("dbo.Files&SubscribedUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Files&SubscribedUsers", "FileId", "dbo.Files");
            DropForeignKey("dbo.Files", "SectionId", "dbo.Sections");
            DropForeignKey("dbo.Files", "OwnerId", "dbo.Users");
            DropForeignKey("dbo.Files&PermittedUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.Files&PermittedUsers", "FileId", "dbo.Files");
            DropIndex("dbo.Users&Roles", new[] {"RoleId"});
            DropIndex("dbo.Users&Roles", new[] {"UserId"});
            DropIndex("dbo.Files&Tags", new[] {"TagId"});
            DropIndex("dbo.Files&Tags", new[] {"FileId"});
            DropIndex("dbo.Files&SubscribedUsers", new[] {"UserId"});
            DropIndex("dbo.Files&SubscribedUsers", new[] {"FileId"});
            DropIndex("dbo.Files&PermittedUsers", new[] {"UserId"});
            DropIndex("dbo.Files&PermittedUsers", new[] {"FileId"});
            DropIndex("dbo.News", new[] {"AuthorId"});
            DropIndex("dbo.Files", new[] {"OwnerId"});
            DropIndex("dbo.Files", new[] {"SectionId"});
            DropIndex("dbo.Downloads", new[] {"UserId"});
            DropIndex("dbo.Downloads", new[] {"FileId"});
            DropIndex("dbo.Comments", new[] {"AuthorId"});
            DropIndex("dbo.Comments", new[] {"FileId"});
            DropTable("dbo.Users&Roles");
            DropTable("dbo.Files&Tags");
            DropTable("dbo.Files&SubscribedUsers");
            DropTable("dbo.Files&PermittedUsers");
            DropTable("dbo.Roles");
            DropTable("dbo.News");
            DropTable("dbo.Tags");
            DropTable("dbo.Sections");
            DropTable("dbo.Files");
            DropTable("dbo.Downloads");
            DropTable("dbo.Users");
            DropTable("dbo.Comments");
        }
    }
}