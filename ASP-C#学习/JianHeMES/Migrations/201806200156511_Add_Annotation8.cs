namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModelCollections", "OrderNum", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ModelCollections", "OrderNum");
        }
    }
}
