namespace JianHeMES.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Annotation7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appearances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNum = c.String(),
                        BarCodesNum = c.String(),
                        OQCCheckBT = c.DateTime(),
                        OQCPrincipal = c.String(),
                        OQCCheckFT = c.DateTime(),
                        OQCCheckTime = c.Time(precision: 7),
                        OQCCheckTimeSpan = c.String(),
                        Appearance_OQCCheckAbnormal = c.Int(nullable: false),
                        RepairCondition = c.String(),
                        OQCCheckFinish = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Appearance_OQCCheckAbnormal",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        DetialedDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Packagings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNum = c.String(),
                        BarCodesNum = c.String(),
                        OQCCheckBT = c.DateTime(),
                        OQCPrincipal = c.String(),
                        OQCCheckFT = c.DateTime(),
                        OQCCheckTime = c.Time(precision: 7),
                        OQCCheckTimeSpan = c.String(),
                        Packaging_OQCCheckAbnormal = c.Int(nullable: false),
                        RepairCondition = c.String(),
                        OQCCheckFinish = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Packaging_OQCCheckAbnormal",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.Int(nullable: false),
                        DetialedDescription = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Assembles", "Appearance_Id", c => c.Int());
            AddColumn("dbo.Assembles", "Packaging_Id", c => c.Int());
            AddColumn("dbo.BarCodes", "Appearance_Id", c => c.Int());
            AddColumn("dbo.BarCodes", "Packaging_Id", c => c.Int());
            AddColumn("dbo.OrderMgms", "Appearance_Id", c => c.Int());
            AddColumn("dbo.OrderMgms", "Packaging_Id", c => c.Int());
            AddColumn("dbo.Users", "Appearance_Id", c => c.Int());
            AddColumn("dbo.Users", "Packaging_Id", c => c.Int());
            AddColumn("dbo.IPQCCheckabnormals", "Appearance_Id", c => c.Int());
            AddColumn("dbo.IPQCCheckabnormals", "Packaging_Id", c => c.Int());
            CreateIndex("dbo.Assembles", "Appearance_Id");
            CreateIndex("dbo.Assembles", "Packaging_Id");
            CreateIndex("dbo.BarCodes", "Appearance_Id");
            CreateIndex("dbo.BarCodes", "Packaging_Id");
            CreateIndex("dbo.OrderMgms", "Appearance_Id");
            CreateIndex("dbo.OrderMgms", "Packaging_Id");
            CreateIndex("dbo.Users", "Appearance_Id");
            CreateIndex("dbo.Users", "Packaging_Id");
            CreateIndex("dbo.IPQCCheckabnormals", "Appearance_Id");
            CreateIndex("dbo.IPQCCheckabnormals", "Packaging_Id");
            AddForeignKey("dbo.Assembles", "Appearance_Id", "dbo.Appearances", "Id");
            AddForeignKey("dbo.BarCodes", "Appearance_Id", "dbo.Appearances", "Id");
            AddForeignKey("dbo.IPQCCheckabnormals", "Appearance_Id", "dbo.Appearances", "Id");
            AddForeignKey("dbo.OrderMgms", "Appearance_Id", "dbo.Appearances", "Id");
            AddForeignKey("dbo.Users", "Appearance_Id", "dbo.Appearances", "Id");
            AddForeignKey("dbo.Assembles", "Packaging_Id", "dbo.Packagings", "Id");
            AddForeignKey("dbo.BarCodes", "Packaging_Id", "dbo.Packagings", "Id");
            AddForeignKey("dbo.IPQCCheckabnormals", "Packaging_Id", "dbo.Packagings", "Id");
            AddForeignKey("dbo.OrderMgms", "Packaging_Id", "dbo.Packagings", "Id");
            AddForeignKey("dbo.Users", "Packaging_Id", "dbo.Packagings", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "Packaging_Id", "dbo.Packagings");
            DropForeignKey("dbo.OrderMgms", "Packaging_Id", "dbo.Packagings");
            DropForeignKey("dbo.IPQCCheckabnormals", "Packaging_Id", "dbo.Packagings");
            DropForeignKey("dbo.BarCodes", "Packaging_Id", "dbo.Packagings");
            DropForeignKey("dbo.Assembles", "Packaging_Id", "dbo.Packagings");
            DropForeignKey("dbo.Users", "Appearance_Id", "dbo.Appearances");
            DropForeignKey("dbo.OrderMgms", "Appearance_Id", "dbo.Appearances");
            DropForeignKey("dbo.IPQCCheckabnormals", "Appearance_Id", "dbo.Appearances");
            DropForeignKey("dbo.BarCodes", "Appearance_Id", "dbo.Appearances");
            DropForeignKey("dbo.Assembles", "Appearance_Id", "dbo.Appearances");
            DropIndex("dbo.IPQCCheckabnormals", new[] { "Packaging_Id" });
            DropIndex("dbo.IPQCCheckabnormals", new[] { "Appearance_Id" });
            DropIndex("dbo.Users", new[] { "Packaging_Id" });
            DropIndex("dbo.Users", new[] { "Appearance_Id" });
            DropIndex("dbo.OrderMgms", new[] { "Packaging_Id" });
            DropIndex("dbo.OrderMgms", new[] { "Appearance_Id" });
            DropIndex("dbo.BarCodes", new[] { "Packaging_Id" });
            DropIndex("dbo.BarCodes", new[] { "Appearance_Id" });
            DropIndex("dbo.Assembles", new[] { "Packaging_Id" });
            DropIndex("dbo.Assembles", new[] { "Appearance_Id" });
            DropColumn("dbo.IPQCCheckabnormals", "Packaging_Id");
            DropColumn("dbo.IPQCCheckabnormals", "Appearance_Id");
            DropColumn("dbo.Users", "Packaging_Id");
            DropColumn("dbo.Users", "Appearance_Id");
            DropColumn("dbo.OrderMgms", "Packaging_Id");
            DropColumn("dbo.OrderMgms", "Appearance_Id");
            DropColumn("dbo.BarCodes", "Packaging_Id");
            DropColumn("dbo.BarCodes", "Appearance_Id");
            DropColumn("dbo.Assembles", "Packaging_Id");
            DropColumn("dbo.Assembles", "Appearance_Id");
            DropTable("dbo.Packaging_OQCCheckAbnormal");
            DropTable("dbo.Packagings");
            DropTable("dbo.Appearance_OQCCheckAbnormal");
            DropTable("dbo.Appearances");
        }
    }
}
