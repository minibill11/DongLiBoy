namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation81 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModelCollections", "StationId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ModelCollections", "StationId");
        }
    }
}