using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    internal class JokerCards
    {
        public string Name { get; set; }
        public string Affect { get; set; }

        public double gameAffect { get; set; }

        public double additionalModifiers { get; set; }
        public int price { get; set; }
        public bool AffectFaceCards { get; set; }
        JokerCards() { }

        public JokerCards(string Name, string Affect, double gameAffect, double additionalModifiers, int price,bool affectFaceCards)
        {
            this.Name = Name;
            this.Affect = Affect;
            this.gameAffect = gameAffect;
            this.additionalModifiers = additionalModifiers;

            this.price = price;
            AffectFaceCards = affectFaceCards;

        }
        override public string ToString()
        {
            return $"CardName {Name} - Effect {Affect} - Chance modifiers {additionalModifiers} - Price{price:c2}";
        }

        public static List<JokerCards> GenerateJokerCards()
        {
            //this is where the joker cards will be generated.
            List<JokerCards> JokerCards = new List<JokerCards>();
            //hardcoded names 
            string[] JokerCardNames = { "Joker of Masks", "Joker of Order", "Joker of Luck", "Joker of Misfortune", "Joker of Power" };
            double[] cardAffect = { 2, 2, 0.5, 20, 100 };//mult effect
            double[] additionalAffect = { 0, 1, 1, 0.05, 0.2 };//chance effect
            int[] price = { 5, 3, 5, 9, 2 };//preset price for each card
            bool[] affectFaceCards = { true, false, false, false,false };

            //this just adds them all to the list of joker cards inside the player class. I could have done this in the constructor but I wanted to keep it separate for ease of use and readability.
            for (int i = 0; i < JokerCardNames.Length; i++)
            {
                JokerCards.Add(new JokerCards(JokerCardNames[i], $"This is the effect of the card {cardAffect[i]}mult", cardAffect[i], additionalAffect[i], price[i], affectFaceCards[i]));
            }

            return JokerCards;

        }
    }
}
