namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.IPQCCheckabnormals", "DetialedDescription", c => c.String());
            AlterColumn("dbo.ViewCheckabnormals", "DetialedDescription", c => c.String());
            AlterColumn("dbo.Waterproofabnormals", "DetialedDescription", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Waterproofabnormals", "DetialedDescription", c => c.Int(nullable: false));
            AlterColumn("dbo.ViewCheckabnormals", "DetialedDescription", c => c.Int(nullable: false));
            AlterColumn("dbo.IPQCCheckabnormals", "DetialedDescription", c => c.Int(nullable: false));
        }
    }
}
