using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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
        //this is to output the cards in a visual sense. 
        public string AskiiArt { get; set; }

        public Cards() { }
        public Cards(string CardName, int CardChipValue, string SuitName, string Effect)
        {
            //simple constructor code 
            this.SuitName = SuitName;
            this.Effect = Effect;
            this.CardChipValue = CardChipValue;

            switch (CardChipValue)
            {
                //changed from the ogironal version as I can then append the card faces 
                //in the txt file. 
                case 1:
                    this.CardName = "A";
                    this.CardChipValue = 14;
                    break;
                case 11:
                    this.CardName = "J";
                    FaceCard = true;
                    this.CardChipValue = CardChipValue;
                    break;
                case 12:
                    this.CardName = "Q";
                    FaceCard = true;
                    this.CardChipValue = CardChipValue;
                    break;
                case 13:
                    this.CardName = "K";
                    FaceCard = true;
                    this.CardChipValue = CardChipValue;
                    break;
                case 14:
                    this.CardName = "A";
                    this.CardChipValue = CardChipValue;
                    break;
                default:
                    //if not a face card then the name is the chipvalue 
                    this.CardName = CardChipValue.ToString();
                    break;
            }
        }
        public override string ToString()
        {
            //ive used the image from the cards folder to generate the 
            //the card image for each spesific card. 
            string cardImageString = $"{SuitName[0]}{CardName}.png";

          return cardImageString;
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



