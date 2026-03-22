using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
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
        //hands and disguards left 
        int handsLeft = 3;
        int disguardsLeft = 3;
        int maxCardsInHand = 5;

        //Loading classes such as deck hands selected cards and cards disguarded. 
        Deck deck = new Deck();
        List<Cards> hand = new List<Cards>();
        List<Cards> selectedHand = new List<Cards>();
        List<Cards> HandPlayed = new List<Cards>();
        List<Cards> HandDiscarded = new List<Cards>();
        List<JokerCards> Jokers = JokerCards.GenerateJokerCards();
        List<JokerCards> JokerCardsInPlay = new List<JokerCards>();
        Player Player = new Player();//I want to add a player class to write to a file. This will track your best score.
        Random random = new Random();


        public MainGamePlayWindow()
        {
            InitializeComponent();
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //creating the deck. it works. 
            deck.CreateDeck();
            //drawing the cards 
            DrawCards(8);
            //refreshing the ui before we start the game. 
            RefreshHandUI();
        }

        private void DrawCards(int amount)
        {
            //this is handy for modular design as it allows me to draw cards
            //and pass a simple parameter into them to take a certian amount. 
            for (int i = 0; i < amount; i++)
            {
                if (deck.FullDeck.Count == 0)
                    return;
                //checking for a new random number inside of the deck
                int newCards = random.Next(0,deck.FullDeck.Count);
                //adding the drawn cards to the deck. 
                Cards drawnCard = deck.FullDeck[newCards];
                hand.Add(drawnCard);
                deck.FullDeck.RemoveAt(newCards);
            }
        }

        //I created a method that Refreshes the card slots once a hand was played.
        private void RefreshHandUI()
        {
            //this is the list of cards that are in the grid. refresing the list when called. 
         List<Button> cardSlots = new List<Button>()
        {HandCard1,HandCard2,HandCard3,HandCard4,HandCard5,HandCard6,HandCard7,HandCard8};

            //this is to add the content to the displayed hand.
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (i < hand.Count)//created a hand that gives the player 10 cards. 
                {
                    cardSlots[i].Content = hand[i].ToString();//simple display of 
                    cardSlots[i].Tag = hand[i];
                    //found a yt tutorial that just shows you how to to do this.
                    cardSlots[i].Background = Brushes.White;

                    //and call these which is wild I spent a good while looking for this. 
                    //when clicking the card it adds it to the clicked method. 
                    cardSlots[i].Click -= Card_Click;
                    cardSlots[i].Click += Card_Click;
                }
                else
                {
                    //this is the edge case if you run out of cards. This is for later in the project. 
                    cardSlots[i].Content = "";
                    cardSlots[i].Tag = null;
                }
            }
        }

        //playing cards method. 
        private void Card_Click(object sender, RoutedEventArgs e)
        {
            //this was a pain to get working. Didnt know that tags were a thing yet they are so helpful.
            Button clickedCard = sender as Button;
            Cards card = clickedCard.Tag as Cards;

            //just in case you try and click on nothing the decide to playsomething. 
            if (card == null)
                return;
            //this is the method to add to you hand. 
            if (selectedHand.Contains(card))
            {
                selectedHand.Remove(card);
                clickedCard.Background = Brushes.White;
                clickedCard.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                if (selectedHand.Count < maxCardsInHand)
                {
                    selectedHand.Add(card);
                    clickedCard.Background = Brushes.AntiqueWhite;
                    clickedCard.Margin = new Thickness(0, -20, 0, 0);
                }
            }
        }
        private void Disguard_Click(object sender, RoutedEventArgs e)
        {
            //more edgecasing. 
            if (selectedHand.Count == 0)
                return;

            //this adds the cards to the disguard pile that will be dumped back into the main deck at the end of round. 
            foreach (var card in selectedHand)
            {
                hand.Remove(card);
                HandDiscarded.Add(card);
            }

            DrawCards(selectedHand.Count);

            foreach (Button btn in CardGrid.Children.OfType<Button>())
            {
                btn.Background = Brushes.White;
                btn.Margin = new Thickness(0,0,0,0);
            }

            selectedHand.Clear();
            RefreshHandUI();

            disguardsLeft--;
        
        }
        private void PlayHand_Click(object sender, RoutedEventArgs e)
        {
            //just in case you try and waste a hand. 
            if (selectedHand.Count == 0)
                return;

            double chipScore = 0;
            double multScore = 0;
            int handNumber = 0;
            bool isStraight = false;

            //had to remove cardsPlayed as im not passing the method into this 
            selectedHand.Sort();

            for (int i = 0; i < selectedHand.Count; i++)
            {
                if (i != 0 && selectedHand[i].CompareTo(selectedHand[i - 1]) == 0)
                {
                    handNumber++;
                }

                chipScore += selectedHand[i].CardChipValue;
            }

            if (selectedHand.Count == 5)
            {
                isStraight = true;

                for (int i = 1; i < selectedHand.Count; i++)
                {
                    if (selectedHand[i].CardChipValue != selectedHand[i - 1].CardChipValue + 1)
                    {
                        isStraight = false;

                        break;
                    }
                }
            }

            if (isStraight)
            {
                chipScore += 55;

                multScore = 5;
            }
            else
            {
                switch (handNumber)
                {
                    case 0:
                        chipScore += 10;
                        multScore = 1;
                        break;

                    case 1:
                        chipScore += 20;
                        multScore = 2;
                        break;

                    case 2:
                        chipScore += 30;
                        multScore = 3;
                        break;

                    case 3:
                        chipScore += 40;
                        multScore = 4;
                        break;

                    case 4:
                        chipScore += 50;
                        multScore = 5;
                        break;
                }
            }

            foreach (var joker in JokerCardsInPlay)
            {
                multScore *= joker.gameAffect;

                chipScore += joker.additionalModifiers;
            }

            foreach (var card in selectedHand)
            {
                hand.Remove(card);

                HandPlayed.Add(card);
            }

            DrawCards(selectedHand.Count);
            //this is here to reset all of the cards that have been played or disguarded. 
            //making the gameplay smoother. I had to scrounge for this .OFTYPE for a bit. 
            foreach (Button btn in CardGrid.Children.OfType<Button>())
            {
                btn.Background = Brushes.White;
                btn.Margin = new Thickness(0, 0, 0, 0);
            }

            selectedHand.Clear();

            RefreshHandUI();

            string finalScore = $"Score: {chipScore * multScore}";

            PlayerChipScore.Text = finalScore;
        }


    }



}