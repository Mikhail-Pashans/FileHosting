namespace FileHosting.Database.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChangeUsersTable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "DownloadAmountLimit", c => c.Decimal(nullable: false, precision: 10, scale: 2));
            AlterColumn("dbo.Users", "DownloadSpeedLimit", c => c.Decimal(nullable: false, precision: 10, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "DownloadSpeedLimit", c => c.Decimal(precision: 10, scale: 2));
            AlterColumn("dbo.Users", "DownloadAmountLimit", c => c.Decimal(precision: 10, scale: 2));
        }
    }
}