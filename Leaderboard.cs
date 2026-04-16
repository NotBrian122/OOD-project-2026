using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    public class Leaderboard
    {
        public int PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int HighScore { get; set; }
        public DateTime Date { get; set; }
    }
   public class HighScoreData : DbContext
    {
        public HighScoreData() : base("LB_Data") 
        { 
            Database.SetInitializer<HighScoreData>(new CreateDatabaseIfNotExists<HighScoreData>());
        }
        public DbSet<Leaderboard> LeaderBoard { get; set; }
    }
}
