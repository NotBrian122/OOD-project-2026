using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    internal class Deck 
    {
        public List<Cards> FullDeck { get; set; }

        public Deck() { }

        public void CreateDeck()
        {
            //adding a new full deck to the list of cards
            FullDeck = new List<Cards>();
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            for (int i = 2; i <= 14; i++)//starts at 2 as theres no 1 in a deck of cards
            {
                foreach (string suit in suits)//foreach will create the loop and deck props 
                {
                    FullDeck.Add(new Cards("", i, suit, ""));//the params and suit will be changed in the Cards class constructor
                }
            }
        }
        //theres room for different game modes and different decks
    }

}

