namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssembleLineIds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LineID = c.String(),
                        AssembleLineName = c.String(),
                        AssembleLineDiscription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModelCollections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BoxBarCode = c.String(),
                        BarCodesNum = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ModelCollections");
            DropTable("dbo.AssembleLineIds");
        }
    }
}
