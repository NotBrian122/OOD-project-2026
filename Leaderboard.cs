using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace OOD_project_2026
{
    public class Leaderboard:Player
    {
        public int PlayerID { get; set; }   
        public string PlayerName { get; set; }
        public int HighScore { get; set; }
        public DateTime Date { get; set; }

    }


    public class HighScoreData : DBContext
    {
        public HighScoreData() : base("LB_Data");
        public DBSet<Leaderboard> LeaderBoard { get; set; }
    }
}
