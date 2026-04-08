using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    public class JokerCards
    {
        public string Name { get; set; }
        public string Affect { get; set; }

        public double gameAffect { get; set; }

        public double additionalModifiers { get; set; }
        public int price { get; set; }
        public bool AffectFaceCards { get; set; }
        public JokerCards() { }

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
            string[] JokerCardNames = { 
                "Joker of Masks",//double mult of the first face card.
                "Joker of Order",//+4 mult for each even card. 
                "Joker of Luck",//increases its own mult everytime 
                "Joker of Misfortune", //+20 mult but every time its activated theres a 1 in 5 chance its destroyed. 
                "Joker of Power", //if a hand is a straight or a flush or a high card then it adds +25 mult .
                "Fantom of Opera"//if you play a face card then it playes it once more. 
            };
            double[] cardAffect = {//mult effect (multliplictive)
                2,//x2 mult
                4,//+4 
                0.5,//+.5 mult every time a lucky card activates.  
                20,//+20 mult  
                25, //+25 mult
                0 //just repeats the card. 
            };
            
            double[] additionalAffect = { //chance affect
                0,//not needed as its a face card. 
                1,//activates every time. 
                1,//ditto
                0.2,//1 in 5 chance
                0.05, //1 in 20 chance
                0 //its activated every time. 
            };//chance effect
            int[] price = { 5, 3, 5, 9, 15 };//preset price for each card
            bool[] affectFaceCards = { true, false, false, false,false };

            //this just adds them all to the list of joker cards inside the player class. I could have done this in the constructor but I wanted to keep it separate for ease of use and readability.
            for (int i = 0; i < JokerCardNames.Length; i++)
            {
                JokerCards.Add(
                    new JokerCards(JokerCardNames[i], 
                    $"This is the effect of the card {cardAffect[i]}mult",
                    cardAffect[i],
                    additionalAffect[i],
                    price[i], 
                    affectFaceCards[i]));
            }

            return JokerCards;

        }
    }
}
