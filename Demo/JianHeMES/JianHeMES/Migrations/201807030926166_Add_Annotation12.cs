namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation12 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Appearances", "Appearance_OQCCheckAbnormal", c => c.String(nullable: false));
            AlterColumn("dbo.Appearances", "RepairCondition", c => c.String(nullable: false));
            AlterColumn("dbo.Burn_in", "Burn_in_OQCCheckAbnormal", c => c.String(nullable: false));
            AlterColumn("dbo.Burn_in", "RepairCondition", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Burn_in", "RepairCondition", c => c.String());
            AlterColumn("dbo.Burn_in", "Burn_in_OQCCheckAbnormal", c => c.String());
            AlterColumn("dbo.Appearances", "RepairCondition", c => c.String());
            AlterColumn("dbo.Appearances", "Appearance_OQCCheckAbnormal", c => c.String());
        }
    }
}
