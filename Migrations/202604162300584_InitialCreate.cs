namespace OOD_project_2026.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HighScoreDatas",
                c => new
                    {
                        PlayerID = c.Int(nullable: false, identity: true),
                        PlayerName = c.String(),
                        HighScore = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PlayerID);

        
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HighScoreDatas");
        }
    }
}
