using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace OOD_project_2026
{
    /// <summary>
    /// Interaction logic for MainGamePlayWindow.xaml
    /// </summary>
    /// 

    //want to load deck and do card gen outside of the main page so everything can work together

    public partial class MainGamePlayWindow : Page
    {
        #region setting variables for the game
        //hands and disguards left 
        int handsLeft = 3;
        int disguardsLeft = 3;
        int maxCardsInHand = 5;
        double playersCurrentScore = 0;
        int blindScore = 300;
        int round = 0;

        //Loading classes such as deck hands selected cards and cards disguarded. 
        Deck deck = new Deck();
        JokerCards JokerCards = new JokerCards();
        List<Cards> hand = new List<Cards>();
        List<Cards> selectedHand = new List<Cards>();
        List<Cards> HandPlayed = new List<Cards>();
        List<Cards> HandDiscarded = new List<Cards>();

        List<JokerCards> JokerCardsInPlay = new List<JokerCards>();
        Player Player = new Player();//I want to add a player class to write to a file. This will track your best score.
        Random random = new Random();


        #endregion
        public MainGamePlayWindow()
        {
            InitializeComponent();
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //creating the deck. it works. 
            deck.CreateDeck();
            //Generating a list of jokers.
            List<JokerCards> Jokers = JokerCards.GenerateJokerCards();
            //drawing the cards 
            DrawCards(8);
            //refreshing the ui before we start the game. 
            RefreshHandUI();
            //Generating blind score 
            round++;
            BlindScoreDisplay.Text = $"BlindScore: {GenerateBlindScore(round)}";
        }

        #region Updating ui/ clard clicking 
        //I created a method that Refreshes the card slots once a hand was played.
        private void RefreshHandUI()
        {
            //this is the list of cards that are in the grid. refresing the list when called. 
            List<Button> cardSlots = new List<Button>()
            {HandCard1,HandCard2,HandCard3,HandCard4,HandCard5,HandCard6,HandCard7,HandCard8};


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
            //updating hands and disguards left
            HandsLeft.Text = $"Hands Left:{handsLeft}";
            DisguardsLeft.Text = $"Disguards Left:{disguardsLeft}";
            //after refreshing the hand ui I do a check for if the player has won or not. 
            CheckWin(playersCurrentScore, handsLeft, disguardsLeft, blindScore);
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
                selectedHand.Remove(card);//if not then it removes 
                clickedCard.Background = Brushes.White;
                clickedCard.Margin = new Thickness(0, 0, 0, 0);//resorts the thickness
            }
            else
            {
                if (selectedHand.Count < maxCardsInHand)//this adds cards to your hand if your less than the max cards in hand.
                {
                    selectedHand.Add(card);
                    //when selecting the card it changes the background colour and the base thickness. 
                    clickedCard.Background = Brushes.AntiqueWhite;
                    clickedCard.Margin = new Thickness(0, -20, 0, 0);
                }
            }
        }
        #endregion
        //disguard cards method. 
        #region Playing or disguarding hands
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
                //this refreshes the children of the grid.
                btn.Background = Brushes.White;
                btn.Margin = new Thickness(0, 0, 0, 0);
            }

            selectedHand.Clear();
            RefreshHandUI();

            disguardsLeft--;

        }
        private void PlayHand_Click(object sender, RoutedEventArgs e)
        {
            //just in case you try and waste a hand. You have to play something to advance the game.
            if (selectedHand.Count == 0)
            {
                return;
            }
                
            //this is for the score, they are doubles as the alrger scores and some other cards
            //can fuck with intagers so Ive started with this. 
            double chipScore = 0, multScore = 0;
            int handNumber = 0;
            string handPlayed = "";//type of hand played
            //checking for staights. 
            bool isFlush = false;
            int isPair = 0;

            //Assinging the card to the CardsPlayedSection 
            List<TextBlock> cardSlotsPlayed = new List<TextBlock>()
            { CardPlayed1,CardPlayed2,CardPlayed3,CardPlayed3,CardPlayed4,CardPlayed5};

            //adding the 2 string to the cards.  
            for (int i = 0; i < selectedHand.Count; i++)
            {
                cardSlotsPlayed[i].Text = selectedHand[i].ToString();//this gives an illusion of cards played. 

            }
            //Wanted to sort the hand before hand as it would make it easier to score. 
            selectedHand.Sort();

            //im going to change these sections here 

            //pair
            if (CheckPair(selectedHand, isPair) == 1)
            {
                handNumber = 1;//this is for a pair 
            }
            // 2 pair 
            else if (CheckPair(selectedHand,isPair) == 2)
            {
                handNumber = 3;//trying to get 2 pair
            //3 of a kind
            }else if (CheckThreeOfAKind(selectedHand)){

                handNumber = 2;//this is for 3 of a kind.
            }
            //stright     
            else if (CheckStriaght(selectedHand))
            {
                handNumber = 4;//I have to check for aces as 2 is below it previously counts as a straight
            }
            //flush.  

            //Then adding the chip score. This is where the animation is going to take palce. 
                

               


                //this for loop is to compare if the selected hands suits a the same. 
                for (int i = 0; i < selectedHand.Count; i++)
                {
                    //this compares the previous card to the current one for the chip value. 
                    //afterwards I have to check for straights
                    //this is for pairs
                    if (i != 0 && selectedHand[i].CompareTo(selectedHand[i - 1]) == 0)
                    {
                        isPair++;//this counts the amount of pairs in your hand. This can include 2 pair. 

                        //this is for edgecases for straights making ace high or low depnding 
                    }
                    
                    //adding the chip score from each of the hands to this. 
                    chipScore += selectedHand[i].CardChipValue;
                }
            #region check for flushes
            //checking if the hand count is 5 for flushes. 
            if (selectedHand.Count == 5)
            {   
                isFlush = true;
                CheckFlush(isFlush, selectedHand);
            }
          
            //this makes straights so much easier to keep a track of. 
            if (isFlush)
            {
                chipScore += 50;
                multScore = 5;
            }
            #endregion
            else
            {
                //else based off of the tallied hand number it chooses what type of hand you played. 
                switch (handNumber)
                {
                    //each of these are hands that have been played. 
                    case 0:
                        //High card
                        handPlayed = "high card";
                        chipScore += 10;
                        multScore = 1;
                        break;
                    case 1:
                        handPlayed = "Pair";
                        chipScore += 20;
                        multScore = 2;
                        break;
                    case 2:
                        handPlayed = "3 of a kind";
                        chipScore += 30;
                        multScore = 3;
                        break;
                    case 3:
                        handPlayed = "Two pair";
                        chipScore += 40;
                        multScore = 4;
                        break;
                    case 4:
                        handPlayed = "Striaght";
                        chipScore += 55;//its harder to get a straight than it is a flush. 
                        multScore = 5;
                        break;
                }
            }
            //this takes the jokers that are played into effect but for now they are unused. 
            foreach (var joker in JokerCardsInPlay)
            {
                multScore *= joker.gameAffect;

                chipScore += joker.additionalModifiers;
            }
            //this removes the card from the hand and puts it in the hand played. 
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
            handsLeft--;
            RefreshHandUI();
            playersCurrentScore += chipScore * multScore;
            //changing the fronend display for the playerscore 
            string finalScore = $"{handPlayed}\nScore: {playersCurrentScore}";
            PlayerChipScore.Text = finalScore;
            CheckWin(playersCurrentScore, handsLeft, disguardsLeft, blindScore);
        }
        #endregion 
        private void DrawCards(int amount)
        {
            //this is handy for modular design as it allows me to draw cards
            //and pass a simple parameter into them to take a certian amount. 
            for (int i = 0; i < amount; i++)
            {
                if (deck.FullDeck.Count == 0)
                    return;
                //checking for a new random number inside of the deck
                int newCards = random.Next(0, deck.FullDeck.Count);
                //adding the drawn cards to the deck. 
                Cards drawnCard = deck.FullDeck[newCards];
                hand.Add(drawnCard);
                deck.FullDeck.RemoveAt(newCards);
            }
        }
        private void CheckWin(double PlayerChipScore, int handsLeft, int disguardsLeft, double BlindScore)
        {
            double comparingScore = BlindScore - PlayerChipScore;
            if (comparingScore <= 0 && handsLeft != 0)
            {
                //you win
                //win window; this will allow you to go to the shop. 

                //resetting current chip score of player.
                playersCurrentScore = 0;
                //im going to put the shop menu window into this.
            }
            else if (comparingScore > 0 && handsLeft == 0)
            {
                //loose as you have no hands left. Restting the game and going back into the game.
            }
            else
            {
                //continue playing aka nothing happens. 
            } 
            //I need to reset blind score after the player is defeated or advance it if they win. 
        }
        private int GenerateBlindScore(int roundScore)
        {
            if (roundScore > 1)
            {
                blindScore += blindScore * roundScore;
                return blindScore;
            }
            else
                return blindScore;
        }
        private bool CheckStriaght(List<Cards> selectedHand)
        {
            bool isStraight = false;
            if (selectedHand.Count == 5)//checking if the hand has 5 cards.
            {
                for (int i = 0; i < selectedHand.Count; i++)
                {
                    //this is for a regular striaght, as your taking away the previous card chip value from the current one 
                    if ((i != 0 && selectedHand[i].CardChipValue - selectedHand[i - 1].CardChipValue == 1)
                        // this is to check for a singualr hand as this edgecase is tough to do with a loop 
                        || (selectedHand[0].CardChipValue == 2
                        && selectedHand[1].CardChipValue == 3
                        && selectedHand[2].CardChipValue == 4
                        && selectedHand[3].CardChipValue == 5
                        && selectedHand[4].CardChipValue == 14))//its a low straight but the chipvalue of the ace is 14 but it also counts as a 1 
                    {
                        isStraight = true;
                        break;
                    }
                }
                return isStraight;
            }
            else
                return isStraight;
        }
        private bool CheckFlush(bool isFlush, List<Cards> selectedHand)
        {
            if (isFlush)
            {
                for (int i = 1; i < selectedHand.Count; i++)
                {
                    //comparing suit names and values to see if you get a flsuh or not 
                    if (i != 0 && selectedHand[i].SuitName != selectedHand[i - 1].SuitName)
                    {
                        isFlush = false;
                       
                    }
                }
            }
            return isFlush;
        }
        private int CheckPair(List<Cards> selectedHand, int isPair)
        {
            for (int i = 0; i < selectedHand.Count; i++)
            {
                if (i != 0 && selectedHand[i].CompareTo(selectedHand[i - 1]) == 0)
                {
                    isPair++;//this counts the amount of pairs in your hand. This can include 2 pair. 
                }
            }
            return isPair;
        }
        private bool CheckThreeOfAKind(List<Cards> selectedHand)
        {
            int isThreeOfAKind = 0;
            for (int i = 0; i < selectedHand.Count; i++)
            {
                if (i != 0 && selectedHand[i].CompareTo(selectedHand[i - 1]) == 0 )
                {
                    isThreeOfAKind++;//this counts the amount of pairs 
                }
            }
            
            if(isThreeOfAKind == 3)
            {
               return true;
            }
            else
                return false;
        }
    }
}