using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    internal class Cards : Deck, IComparable<Cards>
    {
        //obvs name
        public string CardName { get; set; }
        //each card has a "chip value"
        public int CardChipValue { get; set; }
        //Face cards have special effects for certian joker cards
        public bool FaceCard { get; set; }
        //SuitName is obvs 
        public string SuitName { get; set; }
        //Cards can have a modifier so I have to leave this speace clear.
        public string Effect { get; set; }

        public Cards() { }
        public Cards(string CardName, int CardChipValue, string SuitName, string Effect)
        {
            //simple constructor code 
            this.SuitName = SuitName;
            this.Effect = Effect;
            this.CardChipValue = CardChipValue;

            switch (CardChipValue)
            {
                case 1:
                    this.CardName = "Ace";
                    this.CardChipValue = CardChipValue;
                    break;
                case 11:
                    this.CardName = "Jester";
                    FaceCard = true;
                    this.CardChipValue = CardChipValue;
                    break;
                case 12:
                    this.CardName = "Queen";
                    FaceCard = true;
                    this.CardChipValue = CardChipValue;
                    break;
                case 13:
                    this.CardName = "King";
                    FaceCard = true;
                    this.CardChipValue = CardChipValue;
                    break;
                case 14:
                    this.CardName = "Ace";
                    this.CardChipValue = CardChipValue;
                    break;
                default:
                    this.CardName = CardChipValue.ToString();
                    break;
            }
        }
        public override string ToString()
        {
            if (Effect == "")
            {
                return $"{CardName}\n{SuitName}\n{CardChipValue}";
            }
            else
                return $"{CardName}\n{SuitName}\n{CardChipValue}\n{Effect}";
        }
        public void CardEffectAdd()
        {
            string[] effect = { "4Mult", "Glass", "Gold", "Silver", "Lucky" };

        }
        public int CompareTo(Cards other)
        {
            //we start with comparing chip values then suit names 
            if (this.CardChipValue > other.CardChipValue)
            {
                return 1;//non mactching 
            }
            else if (this.CardChipValue < other.CardChipValue)
            {
                return -1;//also non matching 
            }
            else if (this.CardChipValue == other.CardChipValue)
            {
                return 0;//matching card chip value
            }

            else return -1;//presume non matching in order as it makes it clear what to do
            //ive removed the suit comparason as I can do that myself for the suits. 
        }
    }
}



