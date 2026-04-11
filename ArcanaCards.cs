using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    internal class ArcanaCards
    {
        public string CardName { get; set; }
        public string EffectDiscription { get; set; }
        public int NoCardsAffected { get; set; }    

        public string CardAffects { get; set; }
        public List<ArcanaCards> ArcarnaCardsList { get; set; }
        public ArcanaCards() { }

        public List<ArcanaCards> GenreateCarcanraCards()
        {
            string[] CardName = { };
            string[] cardDiscription = { "Select 2 cards to give them this affect \n With this affect is given 4 mult" };
            return ArcarnaCardsList;
        }


    }
}
