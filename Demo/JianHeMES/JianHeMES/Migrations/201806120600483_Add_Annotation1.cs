namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Assembles", "AssembleLineId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Assembles", "AssembleLineId");
        }
    }
}
