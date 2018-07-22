namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Burn_in", "RepairCondition", c => c.String());
            DropColumn("dbo.Burn_in", "repair");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Burn_in", "repair", c => c.String());
            DropColumn("dbo.Burn_in", "RepairCondition");
        }
    }
}
