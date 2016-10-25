namespace ConcurrentTransferMoney.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransferCountToAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "TransferCount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "TransferCount");
        }
    }
}
