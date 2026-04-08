using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.AccessControl;
using System.Security.Policy;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace OOD_project_2026
{
    //want to load deck and do card gen outside of the main page so everything can work together
    public partial class MainGamePlayWindow : Page
    {
        #region setting variables for the game
        //setting up max cards in hands left. 
        int maxCardsInHand = 5;
      //setting up some variables to be used when checking hands, and generating mult. 
            double chipScore = 0;
            double multScore = 0;
            string handPlayed = "";

        //setting the blindscore. 
        int blindScore = 300, round = 0, money = 0;

        //passind that into the current field. 
        double currentChips = 0;
        
        //Loading classes such as deck hands selected cards and cards disguarded. 
        Deck deck = new Deck();
        List<Cards> hand = new List<Cards>();
        List<Cards> selectedHand = new List<Cards>();
        List<Cards> HandPlayed = new List<Cards>();
        List<Cards> HandDiscarded = new List<Cards>();

        List<JokerCards> JokerCardsInPlay = new List<JokerCards>();
        //creating a new list for joker cards to be aded. 
        List<JokerCards> PlayersJokerCardsOwned = new List<JokerCards>();

        //I want to add a player class to write to a file. This will track your best score.
        //I removed the hands and disguards left to the ones in the palyer class to make it better. 
         Random random = new Random();

        List<JokerCards> playersJokerCardsOwned = new List<JokerCards>();
        //its giving out to me for creating the player. 
        Player player;

        #endregion  
        public MainGamePlayWindow()
        {
            InitializeComponent();
            //creating a new player with the default values.
            player = new Player(money, currentChips, playersJokerCardsOwned, 3, 3);
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
            BlindScoreDisplay.Text = $"{GenerateBlindScore(round)}";
        }

        #region Updating ui/ clard clicking 
        //I created a method that Refreshes the card slots once a hand was played.
        private void RefreshHandUI()
        {
            //callign this to reset all card animations. 
            ResetAllCardAnimations();

            //creating a new list of card slots to be played so I can target each card. 
            List<Button> cardSlots = new List<Button>()
            { HandCard1, HandCard2, HandCard3, HandCard4,HandCard5, HandCard6, HandCard7, HandCard8};

            //this is for  a list of images to target, one fo r each card. 
            List<Image> cardImages = new List<Image>()
            { HandImage1, HandImage2, HandImage3, HandImage4,HandImage5, HandImage6, HandImage7, HandImage8 };

            //just resetting chipScoreDisplayUi
            List<TextBlock> chipScoreDisplay = new List<TextBlock>()
            { ChipScore1, ChipScore2, ChipScore3, ChipScore4, ChipScore5 };

            foreach(var chip in chipScoreDisplay)
            {
                chip.Text = "";
            }

            for (int i = 0; i < cardSlots.Count; i++)
            {
                //this targets each cards, for every click of mousehover It targets them and changes some stuff in the background. 
                cardSlots[i].Click -= Card_Click;
                cardSlots[i].MouseEnter -= Card_HoverEnter;
                cardSlots[i].MouseLeave -= Card_HoverLeave;

                cardSlots[i].Visibility = Visibility.Visible;
                cardSlots[i].Tag = null;
                cardSlots[i].BorderBrush = Brushes.Black;
                cardSlots[i].Margin = new Thickness(0);

                cardImages[i].Source = null;

                if (i < hand.Count)//this section tagets cards only when your cards selected is less than the max hand count. 
                {
                    cardSlots[i].Tag = hand[i];
                    cardImages[i].Source = new BitmapImage(
                        new Uri($"pack://application:,,,/Images/Cards/{hand[i]}")
                    );
                    cardImages[i].Stretch = Stretch.Fill;

                    cardSlots[i].Click += Card_Click;
                    cardSlots[i].MouseEnter += Card_HoverEnter;
                    cardSlots[i].MouseLeave += Card_HoverLeave;
                }
            }
            //updaing the disguards and hands left. 
            HandsLeft.Text = $"Hands Left:{player.HandsLeft}";
            DisguardsLeft.Text = $"Disguards Left:{player.DisguardsLeft}";
        }
        //playing cards method. 
        private void Card_Click(object sender, RoutedEventArgs e)
        {
            //creating a list of cards the same as the previous tutorial, made it easier to work with. 
            Button clickedCard = sender as Button;
            Cards card = clickedCard?.Tag as Cards;
            //this is to check the hand and creating varibales for said hand checking. 
            bool isFlush = true;
            int isPair = 0;

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
            switch (CheckHandTypeMain(selectedHand, isFlush, isPair))
            {
                case 0:
                    HandName.Text = "High card";
                    HandChipScore.Text= "10";
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
            if (selectedHand.Count == 0)
            {
                return;
            }
            //some nessesiary variabels to be set 
            bool isFlush = true;
            int isPair = 0;
            //resetting chip and mult score between hands played.  
            chipScore = 0;
            multScore = 0;

            // Keep click order for animation
            List<Cards> playedOrderHand = new List<Cards>(selectedHand);

            // Separate sorted copy for hand scoring logic - making it easier to animatie things and keep it consistant as I 
            //was having some probolems. 
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
            // Score poker hand using sorted copy -- ive moved 
            switch (CheckHandTypeMain(scoringHand, isFlush, isPair))
            {
                case 0:
                    handPlayed = "High card";
                    chipScore += 10;
                    multScore += 1;
                    break;

                case 1:
                    handPlayed = "Pair";
                    chipScore += 20;
                    multScore += 2;
                    break;

                case 2:
                    handPlayed = "3 of a kind";
                    chipScore += 30;
                    multScore += 3;
                    break;

                case 3:
                    handPlayed = "Two pair";
                    chipScore += 40;
                    multScore += 4;
                    break;

                case 4:
                    handPlayed = "Straight";
                    chipScore += 55;
                    multScore += 5;
                    break;

                case 5:
                    handPlayed = "Flush";
                    chipScore += 50;
                    multScore += 5;
                    break;

                case 6:
                    handPlayed = "Full house";
                    chipScore += 40;
                    multScore += 4;
                    break;
            }

           
            // Score each card one by one with pop/rotation
            //Ive added jokers to this level instad of a method as I think its easier to compute them 
            //here before checking the hand type. 
            for (int i = 0; i < playedOrderHand.Count; i++)
            {
                chipScore += playedOrderHand[i].CardChipValue;
                HandChipScore.Text = $"{chipScore}";//updating the front end chipscore
                //now checking every joker card to see if it affects the card
               foreach (JokerCards jokerCards in jokerCardsInEffect)
               {
                   //checking if the card is a face card. 
                   if (playedOrderHand[i].FaceCard && jokerCards.Name == "Joker of Masks")
                   {
                       //this doubles the mult for each face card in play. 
                       multScore *= jokerCards.GameAffect;
                       HandMultScore.Text = $"{multScore}";//updating ui element. 

                   }
                   else if((playedOrderHand[i].CardChipValue % 2 == 0) && jokerCards.Name == "Joker of Order")
                   {
                       multScore += jokerCards.GameAffect;
                   }
             
                  }

                if (i < playedClones.Count)
                {
                    //this is the animaiton function for playing the hand. 
                    //im going to reuse this for joker cards later. 
                    await PopCardWithRotation(playedClones[i]);
                }

                await Task.Delay(60);
            }

            //using a foreach but need to get the index of certian cards. 
       
            if (player.JokerCardsOwned == null)
            {
                return;
            }

           

          

            // Remove played cards from actual hand
            foreach (var card in playedOrderHand)
            {
                hand.Remove(card);
                HandPlayed.Add(card);
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
            if (comparingScore <= 0 && handsLeft != 0)
            {
                //you win
                //win window; this will allow you to go to the shop. 
                WinScreen();
                //resetting current chip score of player.
                player.CurrentChips = 0;
                //im going to put the shop menu window into this.
                //from the win screen. 
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
                blindScore += blindScore * (roundScore%2+1);
                return blindScore;
            }
            else
                return blindScore;
        }
        #endregion
        #region checking hands 
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
        private int CheckPair(List<Cards> selectedHand, int isPair)
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
        private int CheckHandTypeMain(List<Cards> selectedHand, bool isFlush, int isPair)
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
            else if (CheckPair(selectedHand, isPair) == 2)
            {
                handNumber = 3;//trying to get 2 pair

            }
            else if (CheckThreeOfAKind(selectedHand))
            {

                handNumber = 2;//this is for 3 of a kind.
            }
            //stright     
            else if (CheckStriaght(selectedHand))
            {
                handNumber = 4;//I have to check for aces as 2 is below it previously counts as a straight
            }
            //flush.  
            else if (CheckFlush(isFlush, selectedHand))
            {
                handNumber = 5;
            }
            else if (CheckPair(selectedHand, isPair) == 1)
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
            //creating a new translate point based off of the main grid. 
            Point pos = card.TranslatePoint(new Point(0, 0), MainGrid);
            //making the images null, I was told to do this to fix an issue,
            //I had some problems with the images not showing up on the clones.
            Image clonedImage = null;

            if (card.Content is Image originalImage)
            {
                clonedImage = new Image
                {
                    Source = originalImage.Source,
                    Stretch = originalImage.Stretch
                };
            }

            Button clone = new Button
            {
                Width = card.ActualWidth,
                Height = card.ActualHeight,
                Content = clonedImage,
                IsHitTestVisible = false
            };

            Canvas.SetLeft(clone, pos.X);
            Canvas.SetTop(clone, pos.Y);

            Panel.SetZIndex(clone, 999);

            AnimationCanvas.Children.Add(clone);

            return clone;
        } 
        private void ResetAllCardAnimations()
        {
            foreach (Button btn in CardGrid.Children.OfType<Button>())
            {
                var translate = GetCardTranslate(btn);

                translate.BeginAnimation(TranslateTransform.XProperty, null);
                translate.BeginAnimation(TranslateTransform.YProperty, null);

                translate.X = 0;
                translate.Y = 0;

                btn.BorderBrush = Brushes.Black;
                btn.Margin = new Thickness(0);
            }
        }
        #endregion
        #region ShopPopup and logic 
        private async void WinScreen()
        {
            //this is to show the win screen when you win.
            InitialWinScreen.Visibility = Visibility.Visible;
            MainGameplayScreen.IsEnabled = false;

            // counting money earned
            for (int i = 0; i < player.HandsLeft; i++)
            {
                //adding the round and how hard it is. 
                player.Money += round;
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
                await Task.Delay(10);
            }

            RoundScoreDisplay.Text = $"{player.CurrentChips}";

        }
        private void ShowShopVerlay()
        {
            //this is to show the shop overlay when you win. 
            ShopOverlayBackground.Visibility = Visibility.Visible;
            ShopOverLay.Visibility = Visibility.Visible;

            MainGameplayScreen.IsEnabled = false;
            //popup animation
            var fade = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(150)
            };
        }
        private void HideShopOverlay()
        {
            //this is to hide the shop overlay when you win. 
            ShopOverlayBackground.Visibility = Visibility.Collapsed;
            ShopOverLay.Visibility = Visibility.Collapsed;
            MainGameplayScreen.IsEnabled = true;
            //another code block of fading. 
            var fade = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(150)
            };
        }
        private void ContinueFromShop_Click(object sender, RoutedEventArgs e)
        {
            HideShopOverlay();
            round++;
            BlindScoreDisplay.Text =$"{GenerateBlindScore(round)}";
            player = new Player(money, currentChips, playersJokerCardsOwned, 3, 3);

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
            PlayerChipScore.Text = $"Blind Score: {player.CurrentChips}";
        }
        #endregion
    }

}
