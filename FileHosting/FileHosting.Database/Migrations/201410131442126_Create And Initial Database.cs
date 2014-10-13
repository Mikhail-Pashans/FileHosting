namespace FileHosting.Database.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CreateAndInitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comment",
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
                .ForeignKey("dbo.User", t => t.AuthorId)
                .ForeignKey("dbo.File", t => t.FileId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.User",
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
                "dbo.Download",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        FileId = c.Int(nullable: false),
                        UserId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.File", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.FileId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.File",
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
                        IsAllowedAnonymousComments = c.Boolean(nullable: false),
                        SectionId = c.Int(nullable: false),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.User", t => t.OwnerId)
                .ForeignKey("dbo.Section", t => t.SectionId, cascadeDelete: true)
                .Index(t => t.SectionId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.Section",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tag",
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
                .ForeignKey("dbo.User", t => t.AuthorId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FilesAndUsers",
                c => new
                    {
                        FileId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FileId, t.UserId })
                .ForeignKey("dbo.File", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
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
                .ForeignKey("dbo.File", t => t.FileId, cascadeDelete: true)
                .ForeignKey("dbo.Tag", t => t.TagId, cascadeDelete: true)
                .Index(t => t.FileId)
                .Index(t => t.TagId);
            
            CreateTable(
                "dbo.UsersAndRoles",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.User", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Role", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comment", "FileId", "dbo.File");
            DropForeignKey("dbo.Comment", "AuthorId", "dbo.User");
            DropForeignKey("dbo.UsersAndRoles", "RoleId", "dbo.Role");
            DropForeignKey("dbo.UsersAndRoles", "UserId", "dbo.User");
            DropForeignKey("dbo.News", "AuthorId", "dbo.User");
            DropForeignKey("dbo.Download", "UserId", "dbo.User");
            DropForeignKey("dbo.Download", "FileId", "dbo.File");
            DropForeignKey("dbo.FilesAndTags", "TagId", "dbo.Tag");
            DropForeignKey("dbo.FilesAndTags", "FileId", "dbo.File");
            DropForeignKey("dbo.File", "SectionId", "dbo.Section");
            DropForeignKey("dbo.File", "OwnerId", "dbo.User");
            DropForeignKey("dbo.FilesAndUsers", "UserId", "dbo.User");
            DropForeignKey("dbo.FilesAndUsers", "FileId", "dbo.File");
            DropIndex("dbo.UsersAndRoles", new[] { "RoleId" });
            DropIndex("dbo.UsersAndRoles", new[] { "UserId" });
            DropIndex("dbo.FilesAndTags", new[] { "TagId" });
            DropIndex("dbo.FilesAndTags", new[] { "FileId" });
            DropIndex("dbo.FilesAndUsers", new[] { "UserId" });
            DropIndex("dbo.FilesAndUsers", new[] { "FileId" });
            DropIndex("dbo.News", new[] { "AuthorId" });
            DropIndex("dbo.File", new[] { "OwnerId" });
            DropIndex("dbo.File", new[] { "SectionId" });
            DropIndex("dbo.Download", new[] { "UserId" });
            DropIndex("dbo.Download", new[] { "FileId" });
            DropIndex("dbo.Comment", new[] { "AuthorId" });
            DropIndex("dbo.Comment", new[] { "FileId" });
            DropTable("dbo.UsersAndRoles");
            DropTable("dbo.FilesAndTags");
            DropTable("dbo.FilesAndUsers");
            DropTable("dbo.Role");
            DropTable("dbo.News");
            DropTable("dbo.Tag");
            DropTable("dbo.Section");
            DropTable("dbo.File");
            DropTable("dbo.Download");
            DropTable("dbo.User");
            DropTable("dbo.Comment");
        }
    }
}