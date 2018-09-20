namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Burn_in", "BarCodesNum", c => c.String());
            DropColumn("dbo.Burn_in", "BarCodesNumOrderNum");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Burn_in", "BarCodesNumOrderNum", c => c.String());
            DropColumn("dbo.Burn_in", "BarCodesNum");
        }
    }
}
