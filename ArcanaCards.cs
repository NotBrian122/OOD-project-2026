using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace OOD_project_2026
{
    internal class ArcanaCards :JokerCards //The Idea is to inherit the arcana cards from jokers to reuse the same methods which would be handier. 
    {
        public string CardName { get; set; }
        public string EffectDiscription { get; set; }
        public int NoCardsAffected { get; set; }    
        public string Effection { get; set; }
        public int CardPrice { get; set; }

       
        public ArcanaCards() { }

        public ArcanaCards(string cardName, string effectDiscription, int noCardsAffected,string effect,int cardPrice)
        {
            CardName = cardName;
            EffectDiscription = effectDiscription;
            NoCardsAffected = noCardsAffected;
            CardPrice = cardPrice;
            Effection = effect;
        }
        //this is just a generating method to generate all of the cards in said class. 
      
          public static List<ArcanaCards> GenreatearcanraCards()
        {
            return new List<ArcanaCards>
    {
        new ArcanaCards("The Magician", "Enhances up to 2 selected cards into Lucky cards", 2, "Lucky", 3),
        new ArcanaCards("The Empress", "Enhances up to 2 selected cards into Mult cards (+4 mult)", 2, "4Mult", 4),
        new ArcanaCards("The Emperor", "Creates up to 2 random Tarot cards", 2, "Random", 2),
        new ArcanaCards("The Chariot", "Turns 1 selected card into steel", 1, "Silver", 2),
        new ArcanaCards("Justice", "Turns 1 selected card into glass", 1, "Glass", 3),
        new ArcanaCards("The Sun", "Turns up to 3 selected cards into Gold cards", 3, "Gold", 3),
        new ArcanaCards("Hanged Man", "Removes up to 3 selected cards from your hand", 3, "Hanged", 3),
        new ArcanaCards("The Moon", "Turns up to 3 selected cards into clubs", 3, "Clubs", 1),
        new ArcanaCards("The Star", "Turns up to 3 selected cards into hearts", 3, "Hearts", 1),
        new ArcanaCards("The Tower", "Turns up to 3 selected cards into diamonds", 3, "Diamonds", 1),
        new ArcanaCards("The World", "Turns up to 3 selected cards into spades", 3, "Spades", 1)

            };
        }

    }


}

