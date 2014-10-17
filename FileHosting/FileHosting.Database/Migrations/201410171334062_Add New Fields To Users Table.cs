namespace FileHosting.Database.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFieldsToUsersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "DownloadAmountLimit", c => c.Decimal(precision: 10, scale: 2));
            AddColumn("dbo.Users", "DownloadSpeedLimit", c => c.Decimal(precision: 10, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "DownloadSpeedLimit");
            DropColumn("dbo.Users", "DownloadAmountLimit");
        }
    }
}