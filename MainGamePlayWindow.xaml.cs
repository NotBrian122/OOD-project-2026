using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Data.Entity;

namespace OOD_project_2026
{
    //want to load deck and do card gen outside of the main page so everything can work together
    public partial class MainGamePlayWindow : Page
    {
        #region setting variables for the game
        //setting up max cards in hands left. 
        int maxCardsInHand = 5;
        //setting up some variables to be used when checking hands, and generating mult. 
        double chipScore = 299, multScore = 0, currentChips = 0;
        string handPlayed = "";
        bool faceCardPlayed = false;
        //setting the blindscore. 
        int blindScore = 300, round = 0, money = 0;

        //Loading classes such as deck hands selected cards and cards disguarded. 
        Deck deck = new Deck();
        List<Cards> hand = new List<Cards>();
        List<Cards> selectedHand = new List<Cards>();
        List<Cards> HandPlayed = new List<Cards>();
        List<Cards> HandDiscarded = new List<Cards>();

        //creating a new list for joker cards and arcana cards to be aded. 
        List<JokerCards> shopJokers = new List<JokerCards>();
        List<ArcanaCards> shopArcana = new List<ArcanaCards>();

        //I want to add a player class to write to a file. This will track your best score.

        //nothing done here yet. 

        //I removed the hands and disguards left to the ones in the palyer class to make it better. 
        Random random = new Random();
        List<JokerCards> AllJokers = JokerCards.GenerateJokerCards();
        List<JokerCards> playersJokerCardsOwned = new List<JokerCards>();

        //Generating Arcana Cards
        List<ArcanaCards> AllArcanaCards = ArcanaCards.GenreatearcanraCards();
        //creating a list of all arcana cards owned. 
        List<ArcanaCards> playerArcanaCardsOwned = new List<ArcanaCards>();

        //its giving out to me for creating the player. 
        Player player;
        //this was set for changing or selling joker cards. 
        private JokerCards selectedOwnedJokerToSell = null;
        private Button selectedOwnedJokerButton = null;

        //the same for arcana cards. 
        private ArcanaCards selectedArcanaCardsOwned = null;
        private ArcanaCards selecteArcanaCardsToSell = null;
        private Button selectedOwnedArcanaCardButton = null;
        //creating a bool to see if the shop is open or not. 
        private bool isShopOpen = false;
        #endregion  
        public MainGamePlayWindow()
        {
            InitializeComponent();
            //creating a new player with the default values.
            player = new Player(money, currentChips, playersJokerCardsOwned, playerArcanaCardsOwned, 3, 3);
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //creating the deck. it works. 
            deck.CreateDeck();
            //drawing the cards 
            DrawCards(8);
            //refreshing the ui before we start the game. 
            RefreshHandUI();
            //Generating blind score 
            round++;
            BlindScoreDisplay.Text = $"{GenerateBlindScore(round)}";
        }
        //updating ui + card clicking
        #region Updating ui/ clard clicking 
        //I created a method that Refreshes the card slots once a hand was played.
        private void RefreshHandUI()
        {
            ResetAllCardAnimations();
            //cardsot list
            List<Button> cardSlots = new List<Button>()
            {
                 HandCard1, HandCard2, HandCard3, HandCard4,
                 HandCard5, HandCard6, HandCard7, HandCard8
            };
            //jokerslot list. 
            List<Button> jokerSlots = new List<Button>()
            {
                Joker1, Joker2, Joker3, Joker4,
            };

            // Hide Joker and Arcana slots until the player owns some
            foreach (Button btn in JokerCardsOwned.Children.OfType<Button>())
            {
                //intelisense did this, im surpised how powerful it is 
                btn.Visibility = player.JokerCardsOwned.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            //same for arcana cards.
            foreach (Button btn in ArcanaCardsOwned.Children.OfType<Button>())
            {
                btn.Visibility = player.ArcanaCardsOwned.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            for (int i = 0; i < cardSlots.Count; i++)
            {
                //creating a card
                Button currentButton = cardSlots[i];
                //clicking said card. 
                currentButton.Click -= Card_Click;
                currentButton.MouseEnter -= Card_HoverEnter;
                currentButton.MouseLeave -= Card_HoverLeave;

                currentButton.Visibility = Visibility.Visible;
                currentButton.Tag = null;
                currentButton.BorderBrush = Brushes.Black;
                currentButton.Margin = new Thickness(0);
                currentButton.Content = null;

                if (i < hand.Count)
                {
                    Cards currentCard = hand[i];
                    currentButton.Tag = currentCard;

                    Grid cardGrid = new Grid();

                    Image baseImage = new Image
                    {
                        Source = new BitmapImage(
                            new Uri($"pack://application:,,,/Images/Cards/{currentCard}")
                        ),
                        Stretch = Stretch.Fill,
                        IsHitTestVisible = false
                    };

                    cardGrid.Children.Add(baseImage);

                    //addomg card affects to certian cards if they contain said affect. 
                    AddCardEffectOverlay(cardGrid, currentCard);
                    currentButton.Content = cardGrid;

                    //I moved this down here to make it easier  for comprehension
                    currentButton.Click += Card_Click;
                    currentButton.MouseEnter += Card_HoverEnter;
                    currentButton.MouseLeave += Card_HoverLeave;
                }
            }

            HandsLeft.Text = $"Hands Left:{player.HandsLeft}";
            DisguardsLeft.Text = $"Disguards Left:{player.DisguardsLeft}";
        }
        private void AddCardEffectOverlay(Grid cardGrid, Cards card)
        {
            //this is just for testing 
            if (card.Effect == "Lucky")
            {
                //creating a new overlay image. 
                Image overlay = new Image
                {
                    //mapping a new source. 
                    Source = new BitmapImage(
                        new Uri("pack://application:,,,/Images/CardTypeOverlays/Lucky.png")
                    ),
                    Stretch = Stretch.Fill,//I had to make this stretch like the other card
                    IsHitTestVisible = false//making it so I can click said card. 
                };

                cardGrid.Children.Add(overlay);
                //beyond this its copy and paste. 
            }
            else if (card.Effect == "Glass")
            {
                Image overlay = new Image
                {
                    Source = new BitmapImage(
                        new Uri("pack://application:,,,/Images/CardTypeOverlays/Glass.png")
                    ),
                    Stretch = Stretch.Fill,
                    IsHitTestVisible = false
                };

                cardGrid.Children.Add(overlay);

            }
            else if (card.Effect == "Silver")
            {
                Image overlay = new Image
                {
                    Source = new BitmapImage(
                        new Uri("pack://application:,,,/Images/CardTypeOverlays/Silver.png")
                    ),
                    Stretch = Stretch.Fill,
                    IsHitTestVisible = false
                };

                cardGrid.Children.Add(overlay);

            }
            else if (card.Effect == "Gold")
            {
                Image overlay = new Image
                {
                    Source = new BitmapImage(
                        new Uri("pack://application:,,,/Images/CardTypeOverlays/Gold.png")
                    ),
                    Stretch = Stretch.Fill,
                    IsHitTestVisible = false
                };

                cardGrid.Children.Add(overlay);

            }
            else if (card.Effect == "4Mult")
            {
                Image overlay = new Image
                {
                    Source = new BitmapImage(
                        new Uri("pack://application:,,,/Images/CardTypeOverlays/4Mult.png")
                    ),
                    Stretch = Stretch.Fill,
                    IsHitTestVisible = false
                };

                cardGrid.Children.Add(overlay);

            }
        }
        //playing cards method. 
        private void Card_Click(object sender, RoutedEventArgs e)
        {
            //creating a list of cards the same as the previous tutorial, made it easier to work with. 
            Button clickedCard = sender as Button;
            Cards card = clickedCard?.Tag as Cards;//this check the button clicked, sends it off and check if the correct tag clicked card as a card. 
            //this is to check the hand and creating varibales for said hand checking. 
            bool isFlush = true;

            if (card == null)
                return;
            //checking if the hand is selected 
            bool isSelected = selectedHand.Contains(card);

            if (isSelected)
            {
                selectedHand.Remove(card);

                AnimateCard(clickedCard, 0);

                clickedCard.BorderBrush = Brushes.Black;
                clickedCard.Margin = new Thickness(0);
            }
            else
            {
                if (selectedHand.Count >= maxCardsInHand)
                    return;

                selectedHand.Add(card);

                AnimateCard(clickedCard, -25);

                clickedCard.BorderBrush = Brushes.Yellow;
                clickedCard.Margin = new Thickness(0, -20, 0, 0);
            }

            // Using the same method for scoring  but for updating the hands a player could play in this scenario
            switch (CheckHandTypeMain(selectedHand, isFlush))
            {
                case 0:
                    HandName.Text = "High card";
                    HandChipScore.Text = "10";
                    HandMultScore.Text = "1";
                    break;

                case 1:
                    HandName.Text = "Pair";
                    HandChipScore.Text = "20";
                    HandMultScore.Text = "2";
                    break;

                case 2:
                    HandName.Text = "3 of a kind";
                    HandChipScore.Text = "30";
                    HandMultScore.Text = "3";
                    break;

                case 3:
                    HandName.Text = "Two pair";
                    HandChipScore.Text = "40";
                    HandMultScore.Text = "4";
                    break;

                case 4:
                    HandName.Text = "Straight";
                    HandChipScore.Text = "55";
                    HandMultScore.Text = "5";
                    break;

                case 5:
                    HandName.Text = "Flush";
                    HandChipScore.Text = "50";
                    HandMultScore.Text = "5";
                    break;

                case 6:
                    HandName.Text = "Full house";
                    HandChipScore.Text = "40";
                    HandMultScore.Text = "4";
                    break;

            }
        }

        #endregion
        //disguard cards method. 
        #region Playing or disguarding hands
        private async void Disguard_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHand.Count == 0)
                return;

            if (player.DisguardsLeft <= 0)
                return;

            List<Cards> discardedCards = new List<Cards>(selectedHand);

            await AnimateDiscardedHand(discardedCards);

            foreach (var card in discardedCards)
            {
                hand.Remove(card);
                HandDiscarded.Add(card);
            }

            int cardsToReplace = discardedCards.Count;

            selectedHand.Clear();
            player.DisguardsLeft--;
            DisguardsLeft.Text = $"Disguards Left:{player.DisguardsLeft}";
            DrawCards(cardsToReplace);
            RefreshHandUI();

            AnimationCanvas.Children.Clear();
        }
        //I had to change this to an async method for animations. 
        private async void PlayHand_Click(object sender, RoutedEventArgs e)
        {
            //this is to check if your returning null, I had some problems with playtesting. 
            if (selectedHand.Count == 0 || player.HandsLeft <= 0)
            {
                return;
            }
            //some nessesiary
            //variabels to be set 
            bool isFlush = true;
            //resetting the vars here 
            chipScore = 0;
            multScore = 0;
            handPlayed = "";
            faceCardPlayed = false;

            //for lucky cards. 
            Random ranChanceLucky = new Random();


            // Keep click order for animation
            List<Cards> playedOrderHand = new List<Cards>(selectedHand);
            //was having some probolems.copied the scoring hand 
            List<Cards> scoringHand = new List<Cards>(selectedHand);


            //Tried to use this but gave me errors 
            //List<TextBlock> chipScoreDisplay = ChipScoreGird.Children.OfType<TextBlock>().ToList();
            List<TextBlock> chipScoreDisplay = new List<TextBlock>()
            { ChipScore1, ChipScore2, ChipScore3, ChipScore4, ChipScore5 };

            // Animate cards to play area in clicked order to go up to the other gird. 
            List<Button> playedClones = await AnimatePlayedHand(playedOrderHand);

            //creationg a list of jokercards that are owned by the player. 
            List<JokerCards> jokerCardsInEffect = player.JokerCardsOwned.ToList();
            scoringHand.Sort();
            // Score poker hand using sorted copy -- ive moved it as joker cards come after scoring 
            switch (CheckHandTypeMain(scoringHand, isFlush))
            {
                //a lot of this is self explanitory 
                case 0:
                    handPlayed = "High card";
                    chipScore += 10;//changing the chipscore
                    multScore = 1;//chaning the multscore
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
                    handPlayed = "Straight";
                    chipScore += 55;
                    multScore = 5;
                    break;

                case 5:
                    handPlayed = "Flush";
                    chipScore += 50;
                    multScore = 5;
                    break;

                case 6:
                    handPlayed = "Full house";
                    chipScore += 40;
                    multScore = 4;
                    break;
            }

            // Score each card one by one with pop/rotation
            //Ive added jokers to this level instad of a method as I think its easier to compute them 
            //here before checking the hand type. 

            //im going to add the cards left in hand for effect. 
            /*
            foreach (Cards card in scoringHand)
            {
                if (card.Effect == null)
                {
                    return;
                }
                else
                {
                    //luck cards have been accounted for already 
                    switch (card.Effect)
                    {
                        case "4Mult":
                            multScore += 10;
                            break;

                    }
                }

            };
        */
            //ive changed this to make the fantom of opera card to work

            for (int i = 0; i < playedOrderHand.Count && i < playedClones.Count; i++)
            {
                //it affects a siungle card 
                await ReplaySingleCard(playedOrderHand[i], playedClones[i], jokerCardsInEffect, handPlayed, random, true);
            }
            //after this im going game affects to affect the final mult for multiplicitive cards. 
            // trying to get this joker and its spesific Object. 
            var lukyJokerMultAddon = jokerCardsInEffect.FirstOrDefault(jokercards => jokercards.Name == "Joker of Luck");
            if (lukyJokerMultAddon != null)
            {
                multScore *= lukyJokerMultAddon.GameAffect;//mutiplies it afterwards. Lucky cards are broken. 
            }


            // Remove played cards from actual hand
            foreach (var card in playedOrderHand)
            {
                //remving the hand played and then adde it to hand played
                hand.Remove(card);
                HandPlayed.Add(card);
                //adding the cards back into the deck for the next round. 
                deck.FullDeck.Add(card);
            }

            int cardsToReplace = playedOrderHand.Count;

            selectedHand.Clear();
            player.HandsLeft -= 1;

            HandsLeft.Text = $"Hands Left:{player.HandsLeft}";
            //drawing cards and refreshing ui after playing hand.
            DrawCards(cardsToReplace);
            RefreshHandUI();
            //updating player chips and the final score. 
            player.CurrentChips += chipScore * multScore;
            PlayerChipScore.Text = $"{player.CurrentChips}";

            AnimationCanvas.Children.Clear();
            HandName.Text = "";
            HandChipScore.Text = "0";
            HandMultScore.Text = "0";

            CheckWin(player.CurrentChips, player.HandsLeft, player.DisguardsLeft, blindScore);
        }
        #endregion
        #region Main checks and looping 
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
            if (comparingScore <= 0 && player.HandsLeft >= 0)
            {
                // Remove played cards from actual hand
                foreach (var card in HandPlayed.ToList())
                {
                    //removing the palyed hand 
                    HandPlayed.Remove(card);
                    //adding the played hand back into the deck.
                    deck.FullDeck.Add(card);
                }

                //you win
                player.CurrentChips = 0;
                //win window; this will allow you to go to the shop. 
                WinScreen();
                //resetting current chip score of player.
                

                //im going to put the shop menu window into this.
                //from the win screen. 
            }
            else if (comparingScore > 0 && player.HandsLeft == 0)
            {
                //loose as you have no hands left. Restting the game and going back into the game.
                SaveToDBOnFinish();
            }
            else
            {
                //continue playing aka nothing happens. 
                SaveToDBOnFinish();
            }
            //I need to reset blind score after the player is defeated or advance it if they win. 
        }
        private int GenerateBlindScore(int roundScore)
        {
            //if 1 is less than the roundscore
            if (roundScore > 1)
            {
                blindScore += blindScore * (roundScore % 2 + 1);
                return blindScore;
            }
            else
                return blindScore;
        }
        #endregion
        #region checking hands 
        private bool CheckStraight(List<Cards> selectedHand)
        {
            if (selectedHand.Count != 5)
                return false;

            var values = selectedHand
                .Select(c => c.CardChipValue)
                .OrderBy(v => v)
                .ToList();

            // A-2-3-4-5
            if (values.SequenceEqual(new List<int> { 2, 3, 4, 5, 14 }))
                return true;

            for (int i = 1; i < values.Count; i++)
            {
                if (values[i] - values[i - 1] != 1)
                    return false;
            }

            return true;
        }
        private bool CheckFlush(bool isFlush, List<Cards> selectedHand)
        {
            if (isFlush && selectedHand.Count == 5)
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
            else
            {
                isFlush = false;
            }
            return isFlush;
        }
        private int CheckPair(List<Cards> selectedHand)
        {
            //this used to go by my other method but due to so many edgecases 
            //I had to change it via groups. 
            //the reason this method returns an int is that I want to be able to check for 
            //pairs and 2 pairs in the same moethod rather than writing tonnes of code as well. 
            var groups = selectedHand
             //this groups the cards by their chip value 
             .GroupBy(card => card.CardChipValue)
              .Select(g => g.Count()).ToList();
            //to see if its a pair or not.
            if (groups.Count(x => x == 2) == 2)
            {
                return 2;
            }
            else if (groups.Contains(2))
            {
                return 1;
            }
            return 0;
        }
        private bool CheckThreeOfAKind(List<Cards> selectedHand)
        {
            //im going to leave the old code here, theres a really cool tutorial on how to 
            //compare certian hands. Im going to use it for full house 
            /*
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
            */

            //this youtuber used groups for clusters of objects comparing hands
            //from there I applied it to my own work.
            var groups = selectedHand
            //this groups the cards by their chip value 
            .GroupBy(card => card.CardChipValue)
             .Select(g => g.Count()).ToList();

            bool threeOfAKind = false;
            //had to add the last bit for the edge case of a full house as it also containts 3 of a kind. 
            if (groups.Contains(3) && !(groups.Contains(3) && groups.Contains(2)))
            {
                threeOfAKind = true;
            }
            else
            {
                threeOfAKind = false;
            }

            return threeOfAKind;
        }
        private bool CheckFullHouse(List<Cards> selectedHand)
        {
            var groups = selectedHand
            .GroupBy(card => card.CardChipValue)
            .Select(g => g.Count()).ToList();

            bool fullHouse = false;
            if (groups.Contains(3) && groups.Contains(2))
            {
                fullHouse = true;
            }
            else
            {
                fullHouse = false;
            }
            return fullHouse;
        }
        private int CheckHandTypeMain(List<Cards> selectedHand, bool isFlush)
        {
            //the point of this method is a main method ot pass into the other hands
            //checking the hand type and pushing the handNumber out so it makes it easier to 
            //visually process the hand type. 
            int handNumber = 0;//this is for high cards.
            //pair
            if (CheckFullHouse(selectedHand))
            {
                handNumber = 6;//checking full house first 
            }
            // 2 pair 
            else if (CheckPair(selectedHand) == 2)
            {
                handNumber = 3;//trying to get 2 pair

            }
            else if (CheckThreeOfAKind(selectedHand))
            {

                handNumber = 2;//this is for 3 of a kind.
            }
            //stright     
            else if (CheckStraight(selectedHand))
            {
                handNumber = 4;//I have to check for aces as 2 is below it previously counts as a straight
            }
            //flush.  
            else if (CheckFlush(isFlush, selectedHand))
            {
                handNumber = 5;
            }
            else if (CheckPair(selectedHand) == 1)
            {
                handNumber = 1;//this is for a pair 
            }
            else
            {
                return handNumber;
            }
            return handNumber;
        }
        #endregion
        #region JokerCards and caculating affects

        private async Task ApplyJokerEffects(Cards card, Button playedClones, List<JokerCards> jokers, string handPlayed, Random rng, bool allowOpreaReplay)
        {
            int roll = rng.Next(1, 21); // 1..20
            //searching for this joker card, yes it every time but I wanted to make this work
            bool hasRolling = jokers.Any(j => j.Name == "Joker of Rolling");
            int luckySucessRoll = hasRolling ? 19 : 20;//can be either 19 or 20
            int maxRoll = hasRolling ? 10 : 20;//can be either 10 or 20


            //im creating a copy as everything will be removed later on. 
            foreach (var joker in jokers.ToList())
            {
                //Ive added this for joker card animation affects
                Button ownedJokerButton = FindOwnedJokerButton(joker);
                //ive thought a new solution for how im going to do the chipscores. 
                switch (joker.Name)
                {
                    case "Joker of Masks":
                        if (card.FaceCard && !faceCardPlayed)
                        {
                            await PopCardWithRotation(ownedJokerButton);
                            //doublind the score
                            multScore *= joker.GameAffect;
                            //updating the dispaly. 
                            HandMultScore.Text = $"{multScore}";
                            faceCardPlayed = true;//only repeats for the first face card. 
                        }
                        break;
                    case "Joker of Order":
                        if (card.CardChipValue % 2 == 0)
                        {
                            await PopCardWithRotation(ownedJokerButton);
                            multScore += joker.GameAffect;
                            HandMultScore.Text = $"{multScore}";
                        }
                        break;
                    case "Joker of Luck":
                        //checking for a lucky card 
                        if (card.Effect == "Lucky")
                        {
                            roll = rng.Next(1, maxRoll + 1); // 1–20 or could be 1 in 10 

                            if (roll >= luckySucessRoll)//adding the affect as it will change into 1-10 my mistake for chances. 
                            {
                                await PopCardWithRotation(ownedJokerButton);
                                joker.GameAffect += 0.5;//adding .5 to the cards affect. 
                                player.Money += 20;
                                multScore += 20;

                                await PopCardWithRotation(playedClones);
                            }
                            else if (roll == 10 || roll == 15)
                            {
                                await PopCardWithRotation(ownedJokerButton);
                                //just player money
                                player.Money += 20;
                                joker.GameAffect += 0.5;//adding .5 to the cards affect. 

                                await PopCardWithRotation(playedClones);
                            }
                            else if (roll == 14 || roll == 16)
                            {
                                await PopCardWithRotation(ownedJokerButton);
                                //just player multscore.
                                multScore += 20;
                                joker.GameAffect += 0.5;//adding .5 to the cards affect. 

                                await PopCardWithRotation(playedClones);
                            }
                        }
                        break;
                    case "Joker of Misfortune":
                        multScore += joker.GameAffect;
                        HandMultScore.Text = $"{multScore}";

                        //doing the destrying roll as its 1 in 5
                        int destoyRoll = rng.Next(1, 6);
                        if (destoyRoll == 5)
                        {
                            player.JokerCardsOwned.Remove(joker);//removing this spesific joker card. 
                        }
                        break;
                    case "Joker of Power":

                        //this is for higher palying hands. 
                        if (handPlayed == "Flush")
                        {
                            multScore += 5;//adds +5 for each card. thereofre 25 instead of 125
                            await PopCardWithRotation(ownedJokerButton);
                        }
                        else if (handPlayed == "Straight")
                        {
                            multScore += 7;//adds +7 for each card played. therefore 35
                            await PopCardWithRotation(ownedJokerButton);
                        }
                        else if (handPlayed == "High card")
                        {
                            multScore += 45;
                            await PopCardWithRotation(ownedJokerButton);
                        }
                        break;
                    case "Fantom of Opera":
                        if (card.FaceCard && allowOpreaReplay)//checking to make sure its true before changing it 
                        {
                            await ReplaySingleCard(card, playedClones, jokers, handPlayed, rng, false);
                            await PopCardWithRotation(ownedJokerButton);
                        }
                        break;
                    case "Joker of Blood":
                        if (card.SuitName == "Hearts")
                        {
                            int bloodRoll = rng.Next(1, 4); // 1–3
                            if (bloodRoll == 3)
                            {
                                await PopCardWithRotation(ownedJokerButton);
                                multScore *= joker.GameAffect;
                            }
                        }
                        break;


                }
            }
        }
        //this is to replay a single card in the hopes that everything works. 
        private async Task ReplaySingleCard(Cards card, Button playedClone, List<JokerCards> jokers, string handsPlayed, Random rng, bool allowOpreaReplay)
        {
            // base card scoring
            chipScore += card.CardChipValue;
            HandChipScore.Text = $"{chipScore}";

            // normal joker effects for this card
            if (jokers.Count > 0)
            {
                await ApplyJokerEffects(card, playedClone, jokers, handPlayed, rng, allowOpreaReplay);
            }

            await PopCardWithRotation(playedClone);
            await Task.Delay(60);
        }

        #endregion
        #region Cardanimations 
        //creating a transform group to add some animaitons to the cards. 
        private TranslateTransform GetCardTranslate(Button card)
        {
            //creatinga  new transofrm gorup 
            TransformGroup group = card.RenderTransform as TransformGroup;
            //I was having a problem with the transform group being frozen
            //and not being able to add a new translate transform to it so I had to add this check.
            if (group == null || group.IsFrozen)
            {
                //adding the children to said transform group based off of different scaling, rotating and skewing transfomations. 
                group = new TransformGroup();
                group.Children.Add(new ScaleTransform());
                group.Children.Add(new SkewTransform());
                group.Children.Add(new RotateTransform());

                //new translate transform for animations and adding said children to it. 
                TranslateTransform translate = new TranslateTransform();
                group.Children.Add(translate);

                card.RenderTransform = group;
                return translate;//returning said group.
            }
            //adding/cloning the exsisting group of children to the transalte transofrm. 
            TranslateTransform existing = group.Children
                .OfType<TranslateTransform>()
                .FirstOrDefault();

            //if the exsisting children from said transform gorup exsist then they are added to this. 

            if (existing == null || existing.IsFrozen)
            {
                if (existing != null)
                    group.Children.Remove(existing);

                TranslateTransform translate = new TranslateTransform();
                group.Children.Add(translate);
                return translate;
            }

            return existing;
        }

        //so theres 2 groups of animations one for the hover enter and one for the hover leave is to change 
        //animations to enter card aka mouse hovering over
        private void Card_HoverEnter(object sender, MouseEventArgs e)
        {
            //this created a new group, a youtube tutorial was used for this. 
            Button card = sender as Button;
            Cards c = card?.Tag as Cards;

            if (c == null)
                return;

            //So this wont  override selected cards
            if (selectedHand.Contains(c))
                return;

            AnimateCard(card, -10); // small lift
        }
        //resettting animations when the card leaves. 
        private void Card_HoverLeave(object sender, MouseEventArgs e)
        {
            Button card = sender as Button;
            Cards c = card?.Tag as Cards;//its a boolean as I was getting major erros. 

            if (c == null)
                return;

            // If selected, keep it raised
            if (selectedHand.Contains(c))
                return;
            //then it animattes cards based off of a position. 
            AnimateCard(card, 0);
        }
        //this is for the passive animatioins of the cards on the y axis and x axsis when hovering obove them. 
        private void AnimateCard(Button card, double toY, double? toX = null)
        {
            var translate = GetCardTranslate(card);

            var animY = new DoubleAnimation
            {
                To = toY,
                Duration = TimeSpan.FromMilliseconds(400),
                EasingFunction = new PowerEase
                {
                    EasingMode = EasingMode.EaseOut,
                    Power = 2
                }
            };

            translate.BeginAnimation(TranslateTransform.YProperty, animY);

            if (toX.HasValue)
            {
                var animX = new DoubleAnimation
                {
                    To = toX.Value,
                    Duration = TimeSpan.FromMilliseconds(400),
                    EasingFunction = new PowerEase
                    {
                        EasingMode = EasingMode.EaseOut,
                        Power = 2
                    }
                };

                translate.BeginAnimation(TranslateTransform.XProperty, animX);
            }
        }
        private async Task<List<Button>> AnimatePlayedHand(List<Cards> selectedCards)
        {
            //creating a list gorup of cards that are chidlren of type buttona nd adding them to a list.  
            var buttons = CardGrid.Children.OfType<Button>().ToList();
            var targets = GetPlayTargets();
            List<Button> clones = new List<Button>();

            double finalYOffset = 145;   // final resting height above target row
            double overshootAmount = 30; // small lift before settling

            for (int i = 0; i < selectedCards.Count && i < targets.Count; i++)
            {
                Cards card = selectedCards[i];
                //this is to get the list of origional cards and buttons. 
                Button original = buttons.FirstOrDefault(b => b.Tag == card);
                if (original == null)
                    continue;
                //cloning buttons to move to canvas. only if the origional is null aka moved outta hand. 
                Button clone = MoveToCanvas(original);
                clones.Add(clone);

                // Hide original so it looks removed from the hand immediately
                original.Visibility = Visibility.Collapsed;

                Point target = targets[i];

                double finalLeft = target.X;
                double finalTop = target.Y - finalYOffset;

                var animX = new DoubleAnimation
                {
                    To = finalLeft,
                    Duration = TimeSpan.FromMilliseconds(280),
                    EasingFunction = new CubicEase
                    {
                        EasingMode = EasingMode.EaseOut
                    }
                };

                var animY = new DoubleAnimation
                {
                    To = finalTop - overshootAmount,
                    Duration = TimeSpan.FromMilliseconds(280),
                    EasingFunction = new QuadraticEase
                    {
                        EasingMode = EasingMode.EaseOut
                    }
                };

                clone.BeginAnimation(Canvas.LeftProperty, animX);
                clone.BeginAnimation(Canvas.TopProperty, animY);

                await Task.Delay(280);

                var settleY = new DoubleAnimation
                {
                    To = finalTop,
                    Duration = TimeSpan.FromMilliseconds(140),
                    EasingFunction = new BounceEase
                    {
                        Bounces = 2,
                        Bounciness = 2,
                        EasingMode = EasingMode.EaseOut
                    }
                };

                clone.BeginAnimation(Canvas.TopProperty, settleY);

                await Task.Delay(90);
            }

            return clones;

        }
        //I didnt know that points were a thing. A youtube tutorial helped me with creating a new set posiitons on said main grid 
        //there was a bit of research involved. 
        private List<Point> GetPlayTargets()
        {
            //creating a new list of targets bascially the images. 
            List<Image> targets = new List<Image>()
           {PlayedImage1, PlayedImage2, PlayedImage3, PlayedImage4, PlayedImage5};

            //Then a new list of points 
            List<Point> positions = new List<Point>();

            //then for each image you create and map a new point to each. 
            foreach (var img in targets)
            {
                Point pos = img.TranslatePoint(new Point(0, 0), MainGrid);
                positions.Add(pos);
            }

            return positions;
        }
        //this was created due to the help of a youtube tutroial/
        private async Task PopCardWithRotation(Button card)
        {
            //creating a new point in which the cards display their position around. 
            card.RenderTransformOrigin = new Point(0.5, 0.5);

            TransformGroup group = new TransformGroup();//this combines the transofrmes. 
            ScaleTransform scale = new ScaleTransform(1, 1);//makes the ca rds grow
            RotateTransform rotate = new RotateTransform(0);//this is the rotatte group.

            //adding the children to the group. 
            group.Children.Add(scale);
            group.Children.Add(rotate);

            //I had to render a transform on the group applying the transofrm to the cards. 
            card.RenderTransform = group;

            //this changes the new animation of the x 
            var scaleX = new DoubleAnimation
            {
                To = 1,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,//this section makes the card return back to normal,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            //this for the y 
            var scaleY = new DoubleAnimation
            {
                To = 1.15,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,//ditto
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            var rotateAnim = new DoubleAnimation
            {
                To = -20,
                Duration = TimeSpan.FromMilliseconds(80),
                AutoReverse = true,//ditto
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            //this for the rotaion. 
            var rotateAnim2 = new DoubleAnimation
            {
                To = 12,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,//ditto
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            //this is where the animtaions take place. this is scaling
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);
            //this is roatiting. 
            rotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);
            rotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnim2);

            //this is to delay the cards and the animaitons
            await Task.Delay(80);

        }
        private async Task AnimateDiscardedHand(List<Cards> discardedCards)
        {
            //creating a clone of the cards that have been diguarded. based off of the buttons on the card grid.
            var buttons = CardGrid.Children.OfType<Button>().ToList();
            List<Button> clones = new List<Button>();

            // Get deck position relative to MainGrid , aka next to it. 
            Point deckTarget = DeckArea.TranslatePoint(new Point(0, 0), MainGrid);

            // small spread so cards don't stack perfectly
            //much like the game.
            double spread = 12;

            //then using a for loop for each card is diguarded cards to create a 
            for (int i = 0; i < discardedCards.Count; i++)
            {
                Cards card = discardedCards[i];

                //finding the original button of the card that was diguarded.
                //and animatiing that first, also apart of the youtube tutorial. 
                Button original = buttons.FirstOrDefault(b => b.Tag == card);
                if (original == null)
                    continue;

                //mooving the buttons of the clones to the new ones. 
                Button clone = MoveToCanvas(original);
                clones.Add(clone);

                original.Visibility = Visibility.Collapsed;//this is to hide the original card as soon as its diguarded.

                // Slight offset so discarded cards don't all sit exactly on top of each other, this took a min 
                //and another youtube tutorial to figure out how to do.
                double targetX = deckTarget.X + (i * spread);
                double targetY = deckTarget.Y + (i * 4);

                //animation section. 
                var animX = new DoubleAnimation
                {
                    To = targetX,
                    Duration = TimeSpan.FromMilliseconds(260),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
                };

                var animY = new DoubleAnimation
                {
                    To = targetY,
                    Duration = TimeSpan.FromMilliseconds(260),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                clone.BeginAnimation(Canvas.LeftProperty, animX);
                clone.BeginAnimation(Canvas.TopProperty, animY);

                await Task.Delay(60);
            }

            await Task.Delay(220);

            foreach (var clone in clones)
            {
                var fade = new DoubleAnimation
                {
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(120)
                };
                //this took some gooogling and some major errors to figre out.
                clone.BeginAnimation(UIElement.OpacityProperty, fade);
            }

            await Task.Delay(120);
        }
        private Button MoveToCanvas(Button card)
        {
            //had to change this method entierly, this is the creation of a new points based off of the main grid.
            Point pos = card.TranslatePoint(new Point(0, 0), MainGrid);

            object clonedContent = null;//giving a nulll as then you populate the clones from the origonal grid. 

            if (card.Content is Grid originalGrid)
            {
                Grid newGrid = new Grid();

                //this is the major change besides the new grid.
                foreach (UIElement child in originalGrid.Children)//I didnt know this was a thing you could do with a foreach
                {
                    //child is the orgional image. 
                    if (child is Image originalImage)
                    {
                        Image clonedImage = new Image
                        {
                            Source = originalImage.Source,
                            Stretch = originalImage.Stretch,
                            IsHitTestVisible = false
                        };

                        newGrid.Children.Add(clonedImage);//adding it to the new grid. 
                    }
                }

                clonedContent = newGrid;
            }
            else if (card.Content is Image singleImage)//this is for the origional button and its representetive image. 
            {
                clonedContent = new Image
                {
                    Source = singleImage.Source,
                    Stretch = singleImage.Stretch,
                    IsHitTestVisible = false
                };
            }
            //creating a base button clone that has the same width height and content. 
            Button clone = new Button
            {
                Width = card.ActualWidth,
                Height = card.ActualHeight,
                Content = clonedContent,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(clone, pos.X);
            Canvas.SetTop(clone, pos.Y);
            Panel.SetZIndex(clone, 999);//moving the z index to nearly the max height 

            AnimationCanvas.Children.Add(clone);

            return clone;
        }
        private void ResetAllCardAnimations()
        {
            //this is to reset the x and y propertyies of card buttons in the grid. 
            foreach (Button btn in CardGrid.Children.OfType<Button>())
            {
                //creating a new variable to get the translation poinit of a btn . 
                var translate = GetCardTranslate(btn);

                translate.BeginAnimation(TranslateTransform.XProperty, null);
                translate.BeginAnimation(TranslateTransform.YProperty, null);
                //resetting it to 0
                translate.X = 0;
                translate.Y = 0;
                //resetting the border brushes. 
                btn.BorderBrush = Brushes.Black;
                btn.Margin = new Thickness(0);
            }
        }
        //this is for joker card animations 
        //I have to find the buttons firstly to target like the list point. 
        private Button FindOwnedJokerButton(JokerCards joker)
        {
            //this method returns a button of the joker cards I want to target 
            //this allows me to activate the previous method popcardsWithRoataion
            return JokerCardsOwned.Children.OfType<Button>().FirstOrDefault(btn => btn.Tag == joker);
        }
        #endregion
        #region ShopPopup and logic. 
        //popupo of the win screen. 
        private async void WinScreen()
        {
            //this is to show the win screen when you win.
            InitialWinScreen.Visibility = Visibility.Visible;
            MainGameplayScreen.IsEnabled = false;

            // counting money earned
            for (int i = 0; i < player.HandsLeft; i++)
            {
                //adding the round and how hard it is. 
                player.Money += round + 5;
                //then adding hands left as a bonus to the money earned.
                player.Money += player.HandsLeft;
                MoneyEarnedDisplay.Text = $"{player.Money}";
                await Task.Delay(300);
            }
            //showing money earned after the round and the bonus for how many hands you have left.
            MoneyEarnedDisplay.Text = $"{player.Money}";

            // showing round score
            for (int i = 0; i <= player.CurrentChips; i += 10)
            {
                RoundScoreDisplay.Text = $"{i}";
                await Task.Delay(60);
            }

            RoundScoreDisplay.Text = $"{player.CurrentChips}";

        }
        private void ShowShopVerlay()
        {
            //this is to show the shop overlay when you win. 
            ShopOverlayBackground.Visibility = Visibility.Visible;
            ShopOverLay.Visibility = Visibility.Visible;

            //this makes the background not clickable bar the joker cards. 
            MainGameplayScreen.IsHitTestVisible = false;
            OwnedJokerBar.IsHitTestVisible = true;
            JokerCardsOwned.IsHitTestVisible = true;

            //replaced the mainscreen with this instead as I need some parts to be active still
            isShopOpen = true;

            //players money. 
            PlayerMoneyDisplay.Text = $"Money:{player.Money:c2}";

            //getting joker buttons in teh shop and displaying them
            List<Button> jokerSlots = new List<Button>()
             {
                Joker1, Joker2, Joker3, Joker4,
             };

            //getting the list of arcana cards
            List<Button> ArcanaSlots = new List<Button>()
            {
                CardPack1,CardPack2
            };

            //for the joker slots
            for (int i = 0; i < jokerSlots.Count; i++)
            {
                //adding animations for ender or leave. 
                Button currentJokerButton = jokerSlots[i];
                currentJokerButton.MouseEnter -= Joker_Shop_Enter;
                currentJokerButton.MouseLeave -= Joker_Shop_Leave;

                currentJokerButton.MouseEnter += Joker_Shop_Enter;
                currentJokerButton.MouseLeave += Joker_Shop_Leave;

                currentJokerButton.Visibility = Visibility.Visible;
            }
            //for the arcana slots 

            for (int i = 0; i < ArcanaSlots.Count; i++)
            {
                Button currentArcanaButton = ArcanaSlots[i];
                currentArcanaButton.MouseEnter -= Arcana_Shop_Enter;
                currentArcanaButton.MouseLeave -= Arcana_Shop_Leave;

                currentArcanaButton.MouseEnter += Arcana_Shop_Enter;
                currentArcanaButton.MouseLeave += Arcana_Shop_Leave;
            }
            //loading the jokers into teh shop
            LoadJokersIntoShop();
            //loading arcana cards into the shop
            LoadArcanaCardsIntoShop();

        }
        private void HideShopOverlay()
        {
            //this is to hide the shop overlay when you win. 
            ShopOverlayBackground.Visibility = Visibility.Collapsed;
            ShopOverLay.Visibility = Visibility.Collapsed;
            MainGameplayScreen.IsEnabled = true;
            //for when the shop is closed. 
            MainGameplayScreen.IsHitTestVisible = true;
            OwnedJokerBar.IsHitTestVisible = true;
            JokerCardsOwned.IsHitTestVisible = true;

            isShopOpen = false;
            //another code block of fading. 
            var fade = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(150)
            };
        }
        //continue from shop
        private void ContinueFromShop_Click(object sender, RoutedEventArgs e)
        {
            HideShopOverlay();
            round++;
            BlindScoreDisplay.Text = $"{GenerateBlindScore(round)}";

            player.HandsLeft = 3;
            player.DisguardsLeft = 3;
            player.CurrentChips = 0;

        }

        private void ContinueFromWinScreen_Click(object sender, RoutedEventArgs e)
        {
            ShowShopVerlay();

            InitialWinScreen.Visibility = Visibility.Collapsed;
            //changing the player stats for the next round.
            player.CurrentChips = 0;
            player.HandsLeft = 3;
            player.DisguardsLeft = 3;

            //updating player score. 
            PlayerChipScore.Text = $"{player.CurrentChips}";
            HandsLeft.Text = $"Hands Left:{player.HandsLeft}";
            DisguardsLeft.Text = $"Disguards Left:{player.DisguardsLeft}";

        }
        #endregion
        #region JokerCards, animation and some logic
        //loading jokers into the shop.
        private void LoadJokersIntoShop()
        {
            //creating a veriable of all jokers are loaded into the shop if the player doesnt contain them. 
            var availbeJokers = AllJokers.Where(j => !player.JokerCardsOwned.Any(p => p.Name == j.Name)).ToList();
            //removing jokers from said shop. 
            shopJokers.Clear();
            //loop to populate jokes into the shop where its less than the avaiblke joker section. 
            while (shopJokers.Count < 4 && availbeJokers.Count > 0)
            {
                //getting new index. 
                int index = random.Next(availbeJokers.Count);
                shopJokers.Add(availbeJokers[index]);
                availbeJokers.RemoveAt(index);//removes duplicates.

            }
            //assinging jokers to buttons. 
            AssingJokerToButton(Joker1, 0);
            AssingJokerToButton(Joker2, 1);
            AssingJokerToButton(Joker3, 2);
            AssingJokerToButton(Joker4, 3);

        }
        //assinging jokers to buttons. 
        private void AssingJokerToButton(Button button, int index)
        {
            //this is to check if the joker cards arent bought or not. 
            if (index < shopJokers.Count)
            {
                //assinging a joker based off of the index. 
                button.Tag = shopJokers[index];
                button.Content = shopJokers[index].ToString();
                button.IsEnabled = true;
                //changing the content of the button to be the joker card visual.
                button.Visibility = Visibility.Visible;
                button.Content = BuildJokerCardVisual(shopJokers[index]);

            }
            else
            {
                //if theirs no jokers left Ive only made 8 
                button.Tag = null;
                button.Content = "Sold out";
                button.IsEnabled = false;
            }
        }

        //simple animaiton for joker going up and down.
        private void Joker_Shop_Enter(object sender, MouseEventArgs e)
        {
            if (sender is Button jokerCard && jokerCard.Tag is JokerCards joker)
            {
                AnimateCard(jokerCard, -10);
                //showing jokers text when hovered over. 
                JokerHoverText.Text =
                    $"{joker.Name}\n\n{joker.Affect}\n\nChance: {joker.ChanceAffect}\nPrice: {joker.Price:c2}";

                //creatinng a new joker position
                Point pos = jokerCard.TranslatePoint(new Point(0, 0), MainGrid);

                //this is to show the popup and move it based off of the position of the joker card.
                JokerHoverPopup.Visibility = Visibility.Visible;
                Canvas.SetLeft(JokerHoverPopup, pos.X + jokerCard.ActualWidth + 10);
                Canvas.SetTop(JokerHoverPopup, pos.Y);
            }

            Button JokerCard = sender as Button;
            JokerCards jc = JokerCard?.Tag as JokerCards;

            if (jc == null)
                return;

            AnimateCard(JokerCard, -10);
        }
        private void Joker_Shop_Leave(object sender, MouseEventArgs e)
        {
            //getting the same joker card 
            if (sender is Button jokerCard && jokerCard.Tag is JokerCards)
            {
                //hover affect same as the cards. 
                AnimateCard(jokerCard, 0);
                //removing the overlay. 
                JokerHoverPopup.Visibility = Visibility.Collapsed;
            }

            Button JokerCard = sender as Button;
            JokerCards jc = JokerCard?.Tag as JokerCards;

            if (jc == null)
                return;

            //animating the joker cards with affects. 
            AnimateCard(JokerCard, 0);
        }
        //clicking on a joker card once owned to sell them
        private void OwnedJoker_Click(object sender, RoutedEventArgs e)
        {
            if (!isShopOpen)
                return;

            if (sender is Button btn && btn.Tag is JokerCards joker)
            {
                selectedOwnedJokerToSell = joker;
                selectedOwnedJokerButton = btn;

                List<Button> jokerCardDisplay = JokerCardsOwned.Children.OfType<Button>().ToList();
                foreach (Button jokerBtn in jokerCardDisplay)
                {
                    jokerBtn.BorderBrush = Brushes.Black;
                    jokerBtn.BorderThickness = new Thickness(1);
                }

                btn.BorderBrush = Brushes.Yellow;
                btn.BorderThickness = new Thickness(3);

                Point pos = btn.TranslatePoint(new Point(0, 0), OverlayCanvas);

                SellJokerBtn.Tag = joker;
                SellJokerBtn.Visibility = Visibility.Visible;
                Canvas.SetLeft(SellJokerBtn, pos.X);
                Canvas.SetTop(SellJokerBtn, pos.Y + btn.ActualHeight + 5);
            }
        }
        //clicking on the jokerShop
        private void ClickedOnJokerShopCard(object sender, RoutedEventArgs e)
        {
            //I have to pass in the sender as a button to get said index of the joker card. 
            if ((sender is Button clickedButton) && (clickedButton.Tag is JokerCards jokerClicked) && (player.Money >= jokerClicked.Price))
            {
                bool alreadyOwned = player.JokerCardsOwned.Any(j => j.Name == jokerClicked.Name);

                if (!alreadyOwned)
                {
                    player.JokerCardsOwned.Add(jokerClicked);//adding the joker card to the player class list
                    //activating the front end ui
                    AddJokerToGrid();
                    //removing tags and content. 
                    clickedButton.Tag = null;
                    clickedButton.Content = "Bought";
                    clickedButton.IsEnabled = false;
                    //updating the player money and ui
                    player.Money -= jokerClicked.Price;
                    PlayerMoneyDisplay.Text = $"Money:{player.Money:c2}";
                    //collapse the popup
                    JokerHoverPopup.Visibility = Visibility.Collapsed;

                    //checking that there isint more than 5 joker cards in the shop.
                }
                else if (!alreadyOwned && player.JokerCardsOwned.Count > 5)
                {
                    clickedButton.IsEnabled = false;//the player cant have more than 5 buttons. 
                }
            }
        }
        private void AddJokerToGrid()
        {
            //this is just an edgecase
            if (player.JokerCardsOwned == null)
            {
                return;
            }

            //creating a list of joker cards to display. 
            List<Button> JokerCardDisplay = JokerCardsOwned.Children.OfType<Button>().ToList();

            //setting the joker btn to be colapsed 
            SellJokerBtn.Visibility = Visibility.Collapsed;
            selectedOwnedJokerToSell = null;
            selectedOwnedJokerButton = null;

            for (int i = 0; i < JokerCardDisplay.Count; i++)
            {


                Button btn = JokerCardDisplay[i];
                //showing the players owned joker cards on the grid. 
                btn.Visibility = Visibility.Visible;
                // clear old state just in case. 
                btn.Content = null;
                btn.Tag = null;
                //chaning the btn thickness. 
                btn.BorderBrush = Brushes.Black;
                btn.BorderThickness = new Thickness(1);

                //removing old hover events to prevent stacking.
                btn.MouseEnter -= Joker_Owned_Enter;
                btn.MouseLeave -= Joker_Owned_Leave;



                //if the index is less than the amount of joker card owned. 
                if (i < player.JokerCardsOwned.Count)
                {
                    //creating a new btn for selling jokers on the grid. 
                    //based off of the positon of the joker card btn

                    JokerCards joker = player.JokerCardsOwned[i];
                    btn.Tag = joker;
                    btn.Content = BuildJokerCardVisual(joker);

                    //attaching hover. 
                    btn.MouseEnter += Joker_Owned_Enter;
                    btn.MouseLeave += Joker_Owned_Leave;

                }
            }


        }
        //displaying an animation and joker when the mouyse enters or leaves
        private void Joker_Owned_Enter(object sender, MouseEventArgs e)
        {
            if (sender is Button btn && btn.Tag is JokerCards joker)
            {
                selectedOwnedJokerToSell = joker;
                selectedOwnedJokerButton = btn;
                SellJokerBtn.Tag = joker;

                Button sellJokerBtn = SellJokerBtn;

                Point posSell = btn.TranslatePoint(new Point(0, 0), OverlayCanvas);
                sellJokerBtn.Visibility = Visibility.Visible;
                Canvas.SetLeft(sellJokerBtn, posSell.X);
                Canvas.SetTop(sellJokerBtn, posSell.Y + btn.ActualHeight + 5);

                AnimateCard(btn, -10);

                JokerHoverText.Text = $"{joker.Name}\n\n{joker.Affect}\n\nChance: {joker.ChanceAffect}";

                Point pos = btn.TranslatePoint(new Point(0, 0), OverlayCanvas);
                JokerHoverPopup.Visibility = Visibility.Visible;
                Canvas.SetLeft(JokerHoverPopup, pos.X + btn.ActualWidth + 10);
                Canvas.SetTop(JokerHoverPopup, pos.Y);
            }
        }
        private void Joker_Owned_Leave(object sender, MouseEventArgs e)
        {
            if (sender is Button btn && btn.Tag is JokerCards)
            {
                //unanimating the card and removing it 
                AnimateCard(btn, 0);
                //hiding the jokerPopup
                JokerHoverPopup.Visibility = Visibility.Collapsed;
            }
        }
        //sellign the joker card
        private void SellJokerCard_Click(object sender, RoutedEventArgs e)
        {
            //if there isint anything to "sell" then returns 
            if (selectedOwnedJokerToSell == null)
                return;

            //if the jokerCardOwns contains Jokers to selll. 
            if (player.JokerCardsOwned.Contains(selectedOwnedJokerToSell))
            {
                //setting a half price sell value 
                int sellValue = Math.Max(1, selectedOwnedJokerToSell.Price / 2);

                //removed the joker
                player.JokerCardsOwned.Remove(selectedOwnedJokerToSell);
                //adding the sell value to the player. 
                player.Money += sellValue;

                //updating money. 
                PlayerMoneyDisplay.Text = $"Money:{player.Money:c2}";

                //setting it back to null
                selectedOwnedJokerToSell = null;
                selectedOwnedJokerButton = null;

                //collapsing everything 
                SellJokerBtn.Visibility = Visibility.Collapsed;
                JokerHoverPopup.Visibility = Visibility.Collapsed;

                //callig the method add joker to grid. and loading jokers into said shop. 
                AddJokerToGrid();
                LoadJokersIntoShop();
            }
        }
        //mapping images to the joker cards on the shop anmd grid. 
        private Grid BuildJokerCardVisual(JokerCards joker)
        {
            //this is a helper method with updating joker cards and images. 
            Grid jokerGrid = new Grid();

            Image baseImage = new Image
            {
                Source = new BitmapImage(
                    new Uri($"pack://application:,,,/Images/JokerCards/{joker.Name}.png")//the image and name are based off of the same thing
                ),
                Stretch = Stretch.Fill,
                IsHitTestVisible = false//making it clickable but I removed that featrue. 
            };
            //adding the jokers children to the grid. 
            jokerGrid.Children.Add(baseImage);

            //returning the grid. 
            return jokerGrid;
        }

        #endregion
        #region Arcana Cards
        //loading arcana cards into shop
        //most of this area is based off of the work from joker cards. 
        private void LoadArcanaCardsIntoShop()
        {
            //this is a variable much simialr to the joker cards to see if they can load into the shop if the palyer doesnt have them. 
            var avaiblbeArcanaCards = AllArcanaCards.Where(a => !player.ArcanaCardsOwned.Any(c => c.Name == a.Name)).ToList();
            shopArcana.Clear();
            //its the same code thats copied. 
            while (shopArcana.Count < 2 && avaiblbeArcanaCards.Count > 0)
            {
                int index = random.Next(avaiblbeArcanaCards.Count);
                shopArcana.Add(avaiblbeArcanaCards[index]);
                avaiblbeArcanaCards.RemoveAt(index);
            }

            AssingArcanaCardToButton(CardPack1, 0);
            AssingArcanaCardToButton(CardPack2, 1);

        }
        ///assinging the card to the shop button based off of the index. 
        private void AssingArcanaCardToButton(Button button, int index)
        {
            //again this is copied from the joker class. 
            if (index < shopArcana.Count)
            {
                button.Tag = shopArcana[index];
                button.Content = shopArcana[index].ToString();
                button.IsEnabled = true;
                button.Visibility = Visibility.Visible;
                button.Content = BuildArcanaCardVisual(shopArcana[index]);
            }
            else
            {
                button.Tag = null;
                button.Content = "Sold Out";
                button.IsEnabled = false;
            }
        }
        //applying a png to the arcana cards. 
        private Grid BuildArcanaCardVisual(ArcanaCards aranaCards)
        {
            Grid arcanaGrid = new Grid();
            Image baseImage = new Image
            {
                Source = new BitmapImage(
                    new Uri($"pack://application:,,,/Images/ArcanaCards/{aranaCards.CardName}.png")
                ),
                Stretch = Stretch.Fill,
                IsHitTestVisible = false//these arent clickable
            };
            //adding arcana cards to grid, 
            arcanaGrid.Children.Add(baseImage);

            return arcanaGrid;
        }
        //im going back on this branch as I didnt like the messy code that I produced. 
        private void ClickedOnArcanaShopCard(object sender, RoutedEventArgs e)
        {
            if ((sender is Button clickedButton)
                && (clickedButton.Tag is ArcanaCards ArcanaCardClicked)
                && (player.Money >= ArcanaCardClicked.CardPrice))
            {
                if (player.ArcanaCardsOwned.Count <= 2)
                {
                    player.ArcanaCardsOwned.Add(ArcanaCardClicked);
                    AddArcanaCardToGrid();

                    // Mark this specific slot as bought rather than reloading all slots
                    clickedButton.Tag = null;
                    clickedButton.Content = "Bought";
                    clickedButton.IsEnabled = false;

                    player.Money -= ArcanaCardClicked.CardPrice;
                    PlayerMoneyDisplay.Text = $"Money:{player.Money:c2}";
                    ArcanaHoverPopup.Visibility = Visibility.Collapsed;

                    // Remove from shopArcana so it isn't re-offered
                    shopArcana.Remove(ArcanaCardClicked);

                    // Find which slot index this button corresponds to and
                    // refresh only that slot with a new card
                    List<Button> arcanaSlots = new List<Button> { CardPack1, CardPack2 };
                    int boughtIndex = arcanaSlots.IndexOf(clickedButton);

                    if (boughtIndex >= 0)
                    {
                        // Pull a replacement that the player doesn't already own
                        // and isn't already shown in the other slot
                        var available = AllArcanaCards
                            .Where(a => !player.ArcanaCardsOwned.Any(o => o.Name == a.Name)
                                     && !shopArcana.Any(s => s.Name == a.Name))
                            .ToList();

                        if (available.Count > 0)
                        {
                            int index = random.Next(available.Count);
                            ArcanaCards replacement = available[index];
                            shopArcana.Add(replacement);
                            AssingArcanaCardToButton(clickedButton, shopArcana.IndexOf(replacement));
                        }
                        else
                        {
                            // Nothing left to offer in this slot
                            clickedButton.Tag = null;
                            clickedButton.Content = "Sold Out";
                            clickedButton.IsEnabled = false;
                        }
                    }
                }
                else
                {
                    //the hover popup for edgecases. 
                    ArcanaHoverPopup.Visibility = Visibility.Visible;
                    ArcanaHoverText.Text = "Arcana slots full! Use or sell one first.";
                }
            }
        }
        private void AddArcanaCardToGrid()
        {
            //this is just an edgecase
            if (player.ArcanaCardsOwned == null)
            {
                return;
            }

            //creating a list of joker cards to display. 
            List<Button> ArcnaCardDisplay = ArcanaCardsOwned.Children.OfType<Button>().ToList();

            //setting the joker btn to be colapsed 
            SellArcanaBtn.Visibility = Visibility.Collapsed;
            selecteArcanaCardsToSell = null;
            selectedOwnedArcanaCardButton = null;

            for (int i = 0; i < ArcnaCardDisplay.Count; i++)
            {
                Button btn = ArcnaCardDisplay[i];
                btn.Visibility = Visibility.Visible;

                // clear old state just in case. 
                btn.Content = null;
                btn.Tag = null;
                //chaning the btn thickness. 
                btn.BorderBrush = Brushes.Black;
                btn.BorderThickness = new Thickness(1);

                //removing old hover events to prevent stacking.
                btn.MouseEnter -= Arcana_Owned_Enter;
                btn.MouseLeave -= Arcana_Owned_Leave;



                //if the index is less than the amount of joker card owned. 
                if (i < player.ArcanaCardsOwned.Count)
                {
                    //creating a new btn for selling jokers on the grid. 
                    //based off of the positon of the joker card btn

                    ArcanaCards card = player.ArcanaCardsOwned[i];
                    btn.Tag = card;
                    btn.Content = BuildArcanaCardVisual(card);

                    //attaching hover. 
                    btn.MouseEnter += Arcana_Owned_Enter;
                    btn.MouseLeave += Arcana_Owned_Leave;

                }
            }
        }
        private void Arcana_Shop_Enter(object sender, MouseEventArgs e)
        {
            //the exsact same as the joker class
            if (sender is Button btn && btn.Tag is ArcanaCards arcanaCard)
            {
                selectedArcanaCardsOwned = arcanaCard;
                selectedOwnedArcanaCardButton = btn;

                ArcanaHoverText.Visibility = Visibility.Visible;
                ArcanaHoverPopup.Visibility = Visibility.Visible;
                AnimateCard(btn, -10);

                ArcanaHoverText.Text = $"{arcanaCard.Name}\n\n{arcanaCard.EffectDiscription}\n\nNumber of cards Affected:{arcanaCard.NoCardsAffected}\n\n{arcanaCard.CardPrice:C2}";

                Point pos = btn.TranslatePoint(new Point(0, 0), OverlayCanvas);

                //I want to bring this to the other side of the card unlike the joker as 
                //the arcana cards are right on the edge. 
                Canvas.SetLeft(ArcanaHoverPopup, pos.X - (btn.ActualWidth + 125));
                Canvas.SetTop(ArcanaHoverPopup, pos.Y);
            }
        }
        private void Arcana_Shop_Leave(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ArcanaCards)
            {
                //unanimating the card and removing it 
                AnimateCard(btn, 0);
                //hiding the aarcana popup
                ArcanaHoverPopup.Visibility = Visibility.Collapsed;
            }
        }
        private void Sell_Arcana_Card(object sender, RoutedEventArgs e)
        {  //this is just an edgecase
            if (player.ArcanaCardsOwned == null)
            {
                return;
            }

            //creating a list of joker cards to display. 
            List<Button> ArcanaCardDisplay = ArcanaCardsOwned.Children.OfType<Button>().ToList();

            //setting the joker btn to be colapsed 
            SellArcanaBtn.Visibility = Visibility.Collapsed;
            selecteArcanaCardsToSell = null;
            selectedOwnedArcanaCardButton = null;

            for (int i = 0; i < ArcanaCardDisplay.Count; i++)
            {
                Button btn = ArcanaCardDisplay[i];

                // clear old state just in case. 
                btn.Content = null;
                btn.Tag = null;
                //chaning the btn thickness. 
                btn.BorderBrush = Brushes.Black;
                btn.BorderThickness = new Thickness(1);

                //removing old hover events to prevent stacking.
                btn.MouseEnter -= Joker_Owned_Enter;
                btn.MouseLeave -= Joker_Owned_Leave;



                //if the index is less than the amount of joker card owned. 
                if (i < player.ArcanaCardsOwned.Count)
                {
                    //creating a new btn for selling jokers on the grid. 
                    //based off of the positon of the joker card btn

                    ArcanaCards arcanaCard = player.ArcanaCardsOwned[i];
                    btn.Tag = arcanaCard;
                    btn.Content = BuildArcanaCardVisual(arcanaCard);

                    //attaching hover. 
                    btn.MouseEnter += Joker_Owned_Enter;
                    btn.MouseLeave += Joker_Owned_Leave;

                }
            }

        }
        private void Use_ArcanaCard(object sender, RoutedEventArgs e)
        {
            // Use the tracked selected arcana card, not the button's tag
            if (selecteArcanaCardsToSell == null)
            {
                ArcanaHoverPopup.Visibility = Visibility.Visible;
                ArcanaHoverText.Text = "No arcana card selected.";
                return;
            }

            ArcanaCards arcanaClicked = selecteArcanaCardsToSell;

            // Check if any cards are selected
            if (selectedHand.Count == 0)
            {
                ArcanaHoverPopup.Visibility = Visibility.Visible;
                ArcanaHoverText.Text = "Select at least one card first.";
                return;
            }

            // Check if too many cards are selected
            if (selectedHand.Count > arcanaClicked.NoCardsAffected)
            {
                ArcanaHoverPopup.Visibility = Visibility.Visible;
                ArcanaHoverText.Text = $"Select up to {arcanaClicked.NoCardsAffected} card(s).";
                return;
            }

            Debug.WriteLine("Arcana clicked: " + arcanaClicked.CardName);
            Debug.WriteLine("Arcana effect: " + arcanaClicked.Effection);

            // Apply effects to selected cards
            foreach (Cards card in selectedHand.ToList())
            {
                ApplyArcanaToCard(card, arcanaClicked);

                Button handButton = GetButtonForArcCard(card);
                if (handButton != null)
                {
                    handButton.Content = BuildMainCardVisuals(card);
                    handButton.Tag = card;
                }
            }

            // Remove the used arcana card from player's inventory
            player.ArcanaCardsOwned.Remove(arcanaClicked);

            // Clear selection state
            selecteArcanaCardsToSell = null;
            selectedOwnedArcanaCardButton = null;

            // Hide popups
            ArcanaHoverPopup.Visibility = Visibility.Collapsed;
            UseArcanaCardBtn.Visibility = Visibility.Collapsed;
            SellArcanaBtn.Visibility = Visibility.Collapsed;

            // Clear the selected hand after using the arcana
            selectedHand.Clear();

            // Refresh UI
            AddArcanaCardToGrid();
            RefreshHandUI();
        }

        private void Arcana_Owned_Enter(object sender, MouseEventArgs e)
        {
            //again its ripped stright from the joker owned enter method. 
            if (sender is Button btn && btn.Tag is ArcanaCards ArcCard)
            {
                selecteArcanaCardsToSell = ArcCard;
                selectedOwnedArcanaCardButton = btn;
                SellArcanaBtn.Tag = ArcCard;
                UseArcanaCardBtn.Tag = ArcCard;//added this for the use button. 

                Button sellArcBtn = SellArcanaBtn;
                Button useArcBtn = UseArcanaCardBtn;

                //same translate point for the sell button
                Point posSell = btn.TranslatePoint(new Point(0, 0), OverlayCanvas);
                SellArcanaBtn.Visibility = Visibility.Visible;
                Canvas.SetLeft(sellArcBtn, posSell.X);
                Canvas.SetTop(sellArcBtn, posSell.Y + btn.ActualHeight + 5);

                Point posUse = btn.TranslatePoint(new Point(0, 0), OverlayCanvas);
                UseArcanaCardBtn.Visibility = Visibility.Visible;
                //trying to moove this onto the canvas so it shows both buttons for use and sell. 
                Canvas.SetLeft(useArcBtn, posUse.X + 75);
                Canvas.SetTop(useArcBtn, posUse.Y + btn.ActualHeight + 5);

                AnimateCard(btn, -10);
                //I dont want to show Text including price but cards effectedd. 
                ArcanaHoverText.Text = $"Name:{ArcCard.Name}\n\nEffect discription:{ArcCard.EffectDiscription}\n\nNumber of cards effected: {ArcCard.NoCardsAffected}";

                Point pos = btn.TranslatePoint(new Point(0, 0), OverlayCanvas);
                ArcanaHoverPopup.Visibility = Visibility.Visible;
                ArcanaHoverText.Visibility = Visibility.Visible;
                //reversed hover popup as to make it easier to read due to screen edgecase. 
                Canvas.SetLeft(ArcanaHoverPopup, pos.X - (btn.ActualWidth + 55));
                Canvas.SetTop(ArcanaHoverPopup, pos.Y);
            }

        }
        private void Arcana_Owned_Leave(object sender, MouseEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ArcanaCards)
            {
                //unanimating the card and removing it 
                AnimateCard(btn, 0);
                //hiding the jokerPopup
                ArcanaHoverPopup.Visibility = Visibility.Collapsed;
            }
        }

        //getting and returning the card for the button. 
        private Button GetButtonForArcCard(Cards card)
        {
            //creating a new list of handbuttons that have been used. 
            List<Button> handButtons = new List<Button>
             {
                 HandCard1, HandCard2, HandCard3, HandCard4,
                 HandCard5, HandCard6, HandCard7, HandCard8
             };
            //returning the hand buttons for cars that are based of off the target cards. 
            return handButtons.FirstOrDefault(btn => btn.Tag == card);
        }

        private void ApplyArcanaToCard(Cards cards, ArcanaCards arcana)
        {
            switch (arcana.Effection)
            {
                case "Lucky":
                    cards.Effect = "Lucky";
                    break;
                case "4Mult":
                    cards.Effect = "4Mult";
                    break;
                case "Random":
                    //giving 2 new cards to the user. 
                    player.ArcanaCardsOwned.Clear();
                    var avaiblbeArcanaCards = AllArcanaCards.Where(a => !player.ArcanaCardsOwned.Any(c => c.Name == a.Name)).ToList();
                    while (player.ArcanaCardsOwned.Count < 2 && avaiblbeArcanaCards.Count > 0)
                    {
                        int index = random.Next(avaiblbeArcanaCards.Count);
                        player.ArcanaCardsOwned.Add(avaiblbeArcanaCards[index]);
                        avaiblbeArcanaCards.RemoveAt(index);
                    }
                    AddArcanaCardToGrid();
                    break;
                case "Silver":
                    cards.Effect = "Silver";
                    break;
                case "Glass":
                    cards.Effect = "Glass";
                    break;
                case "Hanged":
                    foreach (Cards card in selectedHand.ToList())
                    {
                        hand.Remove(card);
                        deck.FullDeck.Remove(card);
                    }
                    break;
                case "Clubs":
                    //changing the suitname to clubs. 
                    cards.SuitName = "Clubs";
                    break;
                case "Hearts":
                    //Hearts suit
                    cards.SuitName = "Hearts";
                    break;
                case "Diamonds":
                    //diamonds suit
                    cards.SuitName = "Diamonds";
                    break;
                case "Spades":
                    //world
                    cards.SuitName = "Spades";
                    break;
            }
        }
        private Grid BuildMainCardVisuals(Cards card)
        {
            Grid cardGrid = new Grid();

            Image baseImage = new Image
            {
                Source = new BitmapImage(
                 new Uri($"pack://application:,,,/Images/Cards/{card}")
                 ),
                Stretch = Stretch.Fill,
                IsHitTestVisible = false
            };

            cardGrid.Children.Add(baseImage);

            AddCardEffectOverlay(cardGrid, card);

            return cardGrid;
        }
        #endregion

        #region logging player info into a db
        //getting player info, ie name 

        //connecting to the db

        //updating the score for each round, 
        //if the score is higher than the round previous.
        //win screen at round 6. 

        //can go endless mode or quit the game. 
        //I have to log the score here 

        //once failed the screen ends. 
        //displaying playerscore on the screen of the other monitor. 
        //I need todays date/ the date of the run on the screen. 
        #endregion
        
        private void SaveToDBOnFinish()
        {

            HighScoreData db = new HighScoreData();

            Leaderboard newData = new Leaderboard() { PlayerName = "aaa" };

            db.LeaderBoard.Add(newData);
            db.SaveChanges();

            //calling it to a list. 
            var query = db.LeaderBoard.ToList();
        }
        
    }
}
