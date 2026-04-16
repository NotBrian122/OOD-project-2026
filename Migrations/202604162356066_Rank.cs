namespace OOD_project_2026.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rank : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HighScoreDatas", "Rank", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HighScoreDatas", "Rank");
        }
    }
}
