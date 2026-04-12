using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OOD_project_2026
{
    internal class ArcanaCards :JokerCards //The Idea is to inherit the arcana cards from jokers to reuse the same methods which would be handier. 
    {
        public string CardName { get; set; }
        public string EffectDiscription { get; set; }
        public int NoCardsAffected { get; set; }    
       
        public ArcanaCards() { }

        public ArcanaCards(string cardName, string effectDiscription, int noCardsAffected)
        {
            CardName = cardName;
            EffectDiscription = effectDiscription;
            NoCardsAffected = noCardsAffected;
        }
        //this is just a generating method to generate all of the cards in said class. 
        public static List<ArcanaCards> GenreatearcanraCards()
        {
            List<ArcanaCards> ArcanaCards = new List<ArcanaCards>();

            string[] CardName = {
                "The Magician",
                "The Empress",
                "The Emperor",
                "The Chariot", 
                "Justice",
                "Hanged man",
                "The Moon",
                "The Sun", 
                "The Star",
                "ZA WORLDA"
            };
            string[] cardDiscription = { 
                "Enchances 2 selected cards to Lucky Cards",
                "Enchaces 2 selected cards to Mult Cards(+4 mult)",
                "Creates up to 2 random Taroh Cards",
                "Creates 1 selected card into steel\n(If the card is left in the hand, it adds 1.5 X mult to the final score)",
                "Creates 1 card into Glass(x2 Mult but 1 in 4 chance of breaking once played)",
                "Removes up to 3 selected cards from your hand",
                "Turns 3 selected cards to clubs",
                "Turns 3 selected cards to hearts",
                "Turns 3 selected cards to diamonds",
                "Turns 3 selected cards to Spades"
            };
            int[] cardsAffected =
            {
                2,//magician
                2,//emperess
                2,//emperor
                1,//charoit
                1,//Justice
                3,//Hanged Man
                3,//Moon
                3,//Sun
                3,//Star
                3//World
            };

            //genereating the cards and adding them to a list based off of the strings
            //yes they are hard coded but so are the joker cards. 
            for(int i = 0; i < CardName.Length;i++)
            {
                ArcanaCards.Add(
                    new ArcanaCards(CardName[i], cardDiscription[i], cardsAffected[i]));
            }
            return ArcanaCards;
        }


    }
}
