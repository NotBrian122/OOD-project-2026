using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OOD_project_2026
{
    /// <summary>
    /// Interaction logic for MainGamePlayWindow.xaml
    /// </summary>
    /// 

    //want to load deck and do card gen outside of the main page so everything can work together

    public partial class MainGamePlayWindow : Page
    {
        //Declaring the hands that are there /left

         int handsLeft = 3;
         int disguardsLeft = 3;
         int selectedCards = 0;

        //genereating a new deck
        Deck deck = new Deck();
        //generating hands 
        List<Cards> PotentionalHand = new List<Cards>();
        //records the hands played
        List<Cards> HandPlayed = new List<Cards>();
        //records the hands disguarded
        List<Cards> HandDiscarded = new List<Cards>();
        //generates a list of joker cards with spesific modifiers and effects.
        List<JokerCards> Jokers = JokerCards.GenerateJokerCards();

        //generates blank player class - this should be initialzed once 
        Player Player = new Player();
        //creating the deck - all good here. 
        

        public MainGamePlayWindow()
        {
            InitializeComponent();

        }

        private void CardGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //so this is the section in which the random cards are given from the deck.
            //The grid is loaded. 


            Random random = new Random();
            int randomNUmber =random.Next(0,deck.FullDeck.Count);
            //this is the hand list that the player will get. Completely random.
            List<Cards> hand = new List<Cards>();

            //this is for adding random numbers to your hand
            for (int i = 0; i < 10; i++)
            {
                randomNUmber = random.Next(0, deck.FullDeck.Count);
                hand.Add(deck.FullDeck[randomNUmber]);
                //this is to remove from the deck so we dont get duplicates in our hand
                deck.FullDeck.RemoveAt(randomNUmber);
            }


        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //This is where I want to load my cards. 
            

            //load deck 
            deck.CreateDeck();
           
        }
    }
}
