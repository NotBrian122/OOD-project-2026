namespace OOD_project_2026.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<OOD_project_2026.LeaderBoard>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "OOD_project_2026.LeaderBoard";
        }

        protected override void Seed(OOD_project_2026.LeaderBoard context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
