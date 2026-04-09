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

        public double GameAffect { get; set; }

        public int ChanceAffect { get; set; }
        public int Price { get; set; }
        public JokerCards() { }

        public JokerCards(string Name, string CardAffectText, double GameAffect, int ChanceAffect, int price)
        {
            this.Name = Name;
            this.Affect = CardAffectText;
            this.GameAffect = GameAffect;
            this.ChanceAffect = ChanceAffect;

            this.Price = price;
           

        }
        override public string ToString()
        {
            return $"{Name}\nEffect {Affect}\nChance modifiers: {ChanceAffect}\nPrice{Price:c2}";
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
                "Fantom of Opera",//if you play a face card then it playes it once more. 
                "Joker of Blood",//1 in 3 chance of seeing if the mult is increased by 1.5 for each heart card played/ 
                "Joker of Rolling"//doubles odds for everything 
            };
            string[] JokerCardAffectText ={//description of each joker card, making use of it later. 
                "Double mult of the first face card",
                "+4 mult for each even card",
                "Increases its own mult everytime a 'Lucky' card is activated",
                "+20 mult but every time its activated theres a 1 in 5 chance its destroyed",
                "if a hand is a straight or a flush or a high card then it adds +25 mult,+35 mult ,+45 mult",
                "Face Cards are scored for a second time if activated",
                "1 in 3 chance of mutiplying mult by 1.5 times if the card played is hearts. ",
                "Doubles chances of everything happening"
            };
            double[] cardAffect = {//mult effect (multliplictive)
                2,//x2 mult
                4,//+4 
                0.5,//+.5 mult every time a lucky card activates.  
                20,//+20 mult  
                25, //+25 mult
                0, //just repeats the card. 
                1.5,//1.5 times mult for heart cards
                2
            };

            int[] chanceAffect = { //chance affect
                100,//not needed as its a face card. 
                100,//activates every time. 
                100,//ditto
                20,//1 in 5 chance
                100, //100% chance of activating every time. 
                100, //its activated every time. 
                100,//every time. 
                100
            };

            int[] price = { 5, 3, 5, 9, 15, 6,8,10 };//preset price for each card


            //this just adds them all to the list of joker cards inside the player class. I could have done this in the constructor but I wanted to keep it separate for ease of use and readability.
            for (int i = 0; i < JokerCardNames.Length; i++)
            {
                JokerCards.Add(
                    new JokerCards(JokerCardNames[i], JokerCardAffectText[i], cardAffect[i], chanceAffect[i], price[i]));
            } 
            return JokerCards;
        }
    }
}
