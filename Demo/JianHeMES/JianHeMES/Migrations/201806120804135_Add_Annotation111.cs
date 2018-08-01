namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation111 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderMgms", "OrderCreateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderMgms", "OrderCreateDate");
        }
    }
}
