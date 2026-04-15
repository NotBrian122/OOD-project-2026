using System.Collections.Generic;

namespace OOD_project_2026
{
    internal class Player
    {
        //players properties. 
        //hands and disguards left. 
        public int HandsLeft { get; set; }
        public int DisguardsLeft { get; set; }
        public int Money { get; set; }
        public double CurrentChips { get; set; }
        public List<JokerCards> JokerCardsOwned { get; set; }
        public List<ArcanaCards> ArcanaCardsOwned { get; set; }
        public Player() { }
        public Player(int money, double currentChips, List<JokerCards> jokerCardsOwned, List<ArcanaCards> arcanaCardsOwned, int handsLeft, int disguardsLeft)
        {
            this.Money = money;
            this.JokerCardsOwned = jokerCardsOwned;
            this.CurrentChips = currentChips;
            this.HandsLeft = handsLeft;
            this.DisguardsLeft = disguardsLeft;
            this.ArcanaCardsOwned = arcanaCardsOwned;
        }
    }
}
