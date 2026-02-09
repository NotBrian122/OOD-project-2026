using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    internal class Cards :Deck
    {
        //obvs name
        public string CardName { get; set; }
        //each card has a "chip value"
        public int  CardChipValue { get; set; }
        //Face cards have special effects for certian joker cards
        public bool FaceCard { get; set; }
        //SuitName is obvs 
        public  string SuitName { get; set; }
        //Cards can have a modifier so I have to leave this speace clear.
        public string Effect {  get; set; }

        public Cards() { }
        public Cards(string CardName, int CardChipValue, string SuitName, string Effect)
        {

            this.SuitName = SuitName;
            Effect = Effect;

            switch (CardChipValue)
            {
                case 1:
                    this.CardName = "Ace";
                    break;
                case 11:
                   this. CardName = "Jester";
                    FaceCard = true;
                    break;
                case 12:
                    this.CardName = "Queen";
                    FaceCard = true;
                    break;
                case 13:
                    this.CardName = "King";
                    FaceCard = true;
                    break;
                case 14:
                    this.CardName = "Ace";
                    break;
                default:
                    this.CardName = CardChipValue.ToString();
                    break;
            }
        }
        
    }
        
}



