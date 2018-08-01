namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderMgms", "ModelType", c => c.String());
            AddColumn("dbo.OrderMgms", "BoxType", c => c.String());
            AddColumn("dbo.OrderMgms", "PowerType", c => c.String());
            AddColumn("dbo.OrderMgms", "AdapterCardType", c => c.String());
            AddColumn("dbo.Burn_in", "OrderNum", c => c.String());
            AddColumn("dbo.Burn_in", "BarCodesNumOrderNum", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Burn_in", "BarCodesNumOrderNum");
            DropColumn("dbo.Burn_in", "OrderNum");
            DropColumn("dbo.OrderMgms", "AdapterCardType");
            DropColumn("dbo.OrderMgms", "PowerType");
            DropColumn("dbo.OrderMgms", "BoxType");
            DropColumn("dbo.OrderMgms", "ModelType");
        }
    }
}
