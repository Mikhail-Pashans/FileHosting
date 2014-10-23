namespace FileHosting.Database.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddNameFieldToNewsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.News", "Name", c => c.String(nullable: false, maxLength: 300));
        }
        
        public override void Down()
        {
            DropColumn("dbo.News", "Name");
        }
    }
}