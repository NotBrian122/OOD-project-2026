using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    internal class Player
    {
        //players properties. 
        private int _handsLeft = 3;
        private int _disguardsLeft = 3;
        public int HandsLeft
        {
            get { return _handsLeft; }
            set { value = _handsLeft; }

        }
        public int DisguardsLeft
        {
            get { return _disguardsLeft; }
            set { value = _disguardsLeft; }
        }
        public int Money { get; set; }
        public double CurrentChips { get; set; }
        public List<Cards> CardsSelected { get; set; }
        public List<JokerCards> JokerCardsOwned { get; set; }
        public Player() { }
        public Player(int Money, double CurrentChips, List<Cards> CardsSelected, List<JokerCards> JokerCardsOwned)
        {
            this.Money = Money;
            this.CurrentChips = CurrentChips;
            this.CardsSelected = CardsSelected;
            this.JokerCardsOwned = JokerCardsOwned;
        }
    }
}
