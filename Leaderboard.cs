using System;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace OOD_project_2026
{
    public class HighScoreData
    {
        [Key]
        public int PlayerID { get; set; }
        public int Rank { get; set; }
        public string PlayerName { get; set; }
        public int HighScore { get; set; }
        public DateTime Date { get; set; }
        //how many rounds the player has lasted. 
        public int RoundsLasted { get; set; }

    }



    public class LeaderBoard : DbContext
    {
        public LeaderBoard() : base("LB_Data") { }

        public DbSet<HighScoreData> HighScoreData { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HighScoreData>()
                .HasKey(x => x.PlayerID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
