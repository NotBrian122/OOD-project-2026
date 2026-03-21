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
        List<Cards> hand = new List<Cards>();
        //records the hands played
        List<Cards> HandPlayed = new List<Cards>();
        //records the hands disguarded
        List<Cards> HandDiscarded = new List<Cards>();
        //generates a list of joker cards with spesific modifiers and effects.
        List<JokerCards> Jokers = JokerCards.GenerateJokerCards();
        //this is the base list for the joker Cards that need to be played.
        List<JokerCards> JokerCardsInPlay = new List<JokerCards>();
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
            int randomNUmber = random.Next(0, deck.FullDeck.Count);
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
        //heres the akward part. I have to create it so that you can click on other hands then ask to play or remove them .
        private void HandCard1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void PlayHand_Click(object sender, RoutedEventArgs e)
        {

            //this is where the hand will be played and the effects of the cards will be applied.
            string score = "";
            double chipScore = 0;
            double multScore = 0;
            int handNumber = 0;
            string[] cardsPlayed = new string[hand.Count];
            bool isStraight = false;


            hand.Sort();
            //first I want to compare chip values and suits, done in this here.
            for (int i = 0; i < hand.Count; i++)
            {
                if (i != 0 && hand[i].CompareTo(hand[i - 1]) == 0)
                {
                    handNumber++;
                    Console.WriteLine(hand[i].ToString());
                }
                chipScore += hand[i].CardChipValue;
            }

            //this is for figuring out if there is a 
            for (int i = 0; i < hand.Count; i++)
            {
                if (i != 0 && hand.Count == 5)//checks if the handcount is 5 
                {
                    if(hand[i].CardChipValue == hand[i-1].CardChipValue + 1)//comparing the previous chip value to current one 
                    {                                                       //if the score previous is larger by 1 then we keep the stright true
                        isStraight = true;
                    }
                    else
                    {
                        isStraight = false; //if not then we check to see if the straight is true or not. returning a false bool and ignoring it. 
                        break;
                    }
                }
             
            }


            if (isStraight)//if the straight method worked. else use regualr switch statement 
            {
                chipScore += 55;
                multScore = 5;
            }
            else
            {
                //finding the base mult for the hand. 
                //I have to place some if statements in here to check for the different types of arrrangements in spesific hands.
                switch (handNumber)
                {
                    case 0:
                        Console.WriteLine("High Card");
                        chipScore += 10;
                        multScore = 1;
                        break;
                    case 1:
                        //highcard
                        Console.WriteLine("Pair");
                        chipScore += 20;
                        multScore = 2;
                        break;
                    case 2:
                        //three of a kind or full house
                        Console.WriteLine("Three of a Kind");
                        chipScore += 30;
                        multScore = 3;
                        break;
                    case 3:
                        //three of a kind
                        Console.WriteLine("Two Pair");
                        chipScore += 40;
                        multScore = 4;
                        break;
                    case 4:
                        //flush 
                        Console.WriteLine("flush");
                        chipScore += 50;
                        multScore = 5;
                        break;

                }

            }
                

            //next here is going to be a section for the joker cards and how they affect the score.

            if (JokerCardsInPlay != null)
            {
                foreach (var card in JokerCardsInPlay)
                {
                    //this is where the joker card effects will be applied to the score. 
                    multScore *= card.gameAffect;
                    chipScore += card.additionalModifiers;
                }
            }


            //finally returning the score for the hand. Outputting it to the window. 
            string finalScore = string.Format($"Score:{score}  #{chipScore * multScore}");
            PlayerChipScore.Text = finalScore;
        }


    
    }
}

