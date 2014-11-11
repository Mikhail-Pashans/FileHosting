namespace FileHosting.Database.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CreateAndInitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        FullName = c.String(nullable: false, maxLength: 300),
                        Description = c.String(),
                        UploadDate = c.DateTime(nullable: false),
                        Size = c.Long(nullable: false),
                        Path = c.String(nullable: false, maxLength: 300),
                        IsAllowedAnonymousBrowsing = c.Boolean(nullable: false),
                        IsAllowedAnonymousAction = c.Boolean(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.OwnerId)
                .Index(t => t.CategoryId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 70),
                        CreationDate = c.DateTime(nullable: false),
                        DownloadAmountLimit = c.Decimal(nullable: false, precision: 10, scale: 2),
                        DownloadSpeedLimit = c.Decimal(nullable: false, precision: 10, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        PublishDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        FileId = c.Int(nullable: false),
                        AuthorId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Downloads",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        FileId = c.Int(nullable: false),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.FileId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Text = c.String(nullable: false),
                        Picture = c.String(nullable: false, maxLength: 300),
                        PublishDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UsersAndRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.FilesAndAllowedUsers",
                c => new
                    {
                        FileId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FileId, t.UserId })
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.FilesAndSubscribedUsers",
                c => new
                    {
                        FileId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FileId, t.UserId })
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.FilesAndTags",
                c => new
                    {
                        FileId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FileId, t.TagId })
                .ForeignKey("dbo.Files", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FilesAndTags", "TagId", "dbo.Tags");
            DropForeignKey("dbo.FilesAndTags", "FileId", "dbo.Files");
            DropForeignKey("dbo.FilesAndSubscribedUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.FilesAndSubscribedUsers", "FileId", "dbo.Files");
            DropForeignKey("dbo.Files", "OwnerId", "dbo.Users");
            DropForeignKey("dbo.Files", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.FilesAndAllowedUsers", "UserId", "dbo.Users");
            DropForeignKey("dbo.FilesAndAllowedUsers", "FileId", "dbo.Files");
            DropForeignKey("dbo.UsersAndRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.UsersAndRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.News", "AuthorId", "dbo.Users");
            DropForeignKey("dbo.Downloads", "UserId", "dbo.Users");
            DropForeignKey("dbo.Downloads", "FileId", "dbo.Files");
            DropForeignKey("dbo.Comments", "FileId", "dbo.Files");
            DropForeignKey("dbo.Comments", "AuthorId", "dbo.Users");
            DropIndex("dbo.FilesAndTags", new[] { "TagId" });
            DropIndex("dbo.FilesAndTags", new[] { "FileId" });
            DropIndex("dbo.FilesAndSubscribedUsers", new[] { "UserId" });
            DropIndex("dbo.FilesAndSubscribedUsers", new[] { "FileId" });
            DropIndex("dbo.FilesAndAllowedUsers", new[] { "UserId" });
            DropIndex("dbo.FilesAndAllowedUsers", new[] { "FileId" });
            DropIndex("dbo.UsersAndRoles", new[] { "RoleId" });
            DropIndex("dbo.UsersAndRoles", new[] { "UserId" });
            DropIndex("dbo.News", new[] { "AuthorId" });
            DropIndex("dbo.Downloads", new[] { "UserId" });
            DropIndex("dbo.Downloads", new[] { "FileId" });
            DropIndex("dbo.Comments", new[] { "AuthorId" });
            DropIndex("dbo.Comments", new[] { "FileId" });
            DropIndex("dbo.Files", new[] { "OwnerId" });
            DropIndex("dbo.Files", new[] { "CategoryId" });
            DropTable("dbo.FilesAndTags");
            DropTable("dbo.FilesAndSubscribedUsers");
            DropTable("dbo.FilesAndAllowedUsers");
            DropTable("dbo.UsersAndRoles");
            DropTable("dbo.Tags");
            DropTable("dbo.Roles");
            DropTable("dbo.News");
            DropTable("dbo.Downloads");
            DropTable("dbo.Comments");
            DropTable("dbo.Users");
            DropTable("dbo.Files");
            DropTable("dbo.Categories");
        }
    }
}