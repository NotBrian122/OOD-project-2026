using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOD_project_2026
{
    internal class JokerCards
    {
        public int Weight { get; set; }
        //the weight sets the spesific value for joker cards
        //heaver the weight the more powerful the card is
        public string Name { get; set; }//all joker cards have names

        public string ModifierDiscription { get; set; }//they all have a modifier discription that will be used in the game as a hover effect

        public string JokerEffect { get; set; }//Joker cards can have buffs and defuffs randomly, ie negetive allows for another position 

        public double GameModifier { get; set; }//this is the value that will be used to calculate the effect of the joker card in the game

        public JokerCards() { }
        public JokerCards(int Weight, string Name, string ModifierDiscription, string JokerEffect, double GameModifier)
        {
            this.Weight = Weight;
            this.Name = Name;
            this.ModifierDiscription = ModifierDiscription;
            this.JokerEffect = JokerEffect;
            this.GameModifier = GameModifier;
        }
        override public string ToString()
        {
            return $"{Name} (Weight: {Weight}, Effect: {JokerEffect}, Modifier: {ModifierDiscription}, Game Modifier: {GameModifier})";
        }
    }
}
