namespace FileHosting.Database.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CreateAndInitialDatabase : DbMigration
    {
        public override void Up()
        {
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
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 70),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
                "dbo.Files",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        FullName = c.String(nullable: false, maxLength: 300),
                        Description = c.String(),
                        UploadDate = c.DateTime(nullable: false),
                        Size = c.Decimal(nullable: false, precision: 10, scale: 2),
                        Path = c.String(nullable: false, maxLength: 300),
                        IsAllowedAnonymousBrowsing = c.Boolean(nullable: false),
                        IsAllowedAnonymousAction = c.Boolean(nullable: false),
                        SectionId = c.Int(nullable: false),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.OwnerId)
                .ForeignKey("dbo.Sections", t => t.SectionId, cascadeDelete: true)
                .Index(t => t.SectionId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Sections",
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
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
                "dbo.Files&PermittedUsers",
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
                "dbo.Files&SubscribedUsers",
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
                "dbo.Files&Tags",
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
            
            CreateTable(
                "dbo.Users&Roles",
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
            DropIndex("dbo.Users&Roles", new[] { "RoleId" });
            DropIndex("dbo.Users&Roles", new[] { "UserId" });
            DropIndex("dbo.Files&Tags", new[] { "TagId" });
            DropIndex("dbo.Files&Tags", new[] { "FileId" });
            DropIndex("dbo.Files&SubscribedUsers", new[] { "UserId" });
            DropIndex("dbo.Files&SubscribedUsers", new[] { "FileId" });
            DropIndex("dbo.Files&PermittedUsers", new[] { "UserId" });
            DropIndex("dbo.Files&PermittedUsers", new[] { "FileId" });
            DropIndex("dbo.News", new[] { "AuthorId" });
            DropIndex("dbo.Files", new[] { "OwnerId" });
            DropIndex("dbo.Files", new[] { "SectionId" });
            DropIndex("dbo.Downloads", new[] { "UserId" });
            DropIndex("dbo.Downloads", new[] { "FileId" });
            DropIndex("dbo.Comments", new[] { "AuthorId" });
            DropIndex("dbo.Comments", new[] { "FileId" });
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