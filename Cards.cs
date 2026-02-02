using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    internal class Cards
    {
        public string CardName { get; set; }
        public int  CardChipValue { get; set; }
        public bool FaceCard { get; set; }

        public Cards(string cardName, int cardChipValue, bool faceCard)
        {
            CardName = cardName;
            CardChipValue = cardChipValue;
            FaceCard = faceCard;
        }


        public override string ToString()
        {
            return $"{CardName} - Value: {CardChipValue}, Face Card: {FaceCard}";
        }

    }
}
