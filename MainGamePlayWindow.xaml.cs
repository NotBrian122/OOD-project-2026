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
using System.Windows.Media.Animation;
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
        //initialising joker cards for the shop. 
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
            ResetAllCardAnimations();

            List<Button> cardSlots = new List<Button>()
    {
        HandCard1, HandCard2, HandCard3, HandCard4,
        HandCard5, HandCard6, HandCard7, HandCard8
    };

            List<Image> cardImages = new List<Image>()
    {
        HandImage1, HandImage2, HandImage3, HandImage4,
        HandImage5, HandImage6, HandImage7, HandImage8
    };

            for (int i = 0; i < cardSlots.Count; i++)
            {
                // Always fully reset the slot first
                cardSlots[i].Click -= Card_Click;
                cardSlots[i].MouseEnter -= Card_HoverEnter;
                cardSlots[i].MouseLeave -= Card_HoverLeave;

                cardSlots[i].Tag = null;
                cardSlots[i].BorderBrush = Brushes.Black;
                cardSlots[i].Margin = new Thickness(0);

                // IMPORTANT: clear old image
                cardImages[i].Source = null;

                if (i < hand.Count)
                {
                    cardSlots[i].Tag = hand[i];

                    cardImages[i].Source = new BitmapImage(
                        new Uri($"pack://application:,,,/Images/Cards/{hand[i].ToString()}")
                    );
                    cardImages[i].Stretch = Stretch.Fill;

                    cardSlots[i].Click += Card_Click;
                    cardSlots[i].MouseEnter += Card_HoverEnter;
                    cardSlots[i].MouseLeave += Card_HoverLeave;
                }
                else
                {
                    cardImages[i].Source = null;
                }
            }

            HandsLeft.Text = $"Hands Left:{handsLeft}";
            DisguardsLeft.Text = $"Disguards Left:{disguardsLeft}";

            CheckWin(playersCurrentScore, handsLeft, disguardsLeft, blindScore);
        }
        //playing cards method. 
        private void Card_Click(object sender, RoutedEventArgs e)
        {
            Button clickedCard = sender as Button;
            Cards card = clickedCard?.Tag as Cards;

            if (card == null)
                return;

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
        }

        #endregion
        //disguard cards method. 
        #region Playing or disguarding hands
        private void Disguard_Click(object sender, RoutedEventArgs e)
        {
            if (selectedHand.Count == 0)
                return;

            int cardsToReplace = selectedHand.Count;

            foreach (var card in selectedHand)
            {
                hand.Remove(card);
                HandDiscarded.Add(card);
            }

            selectedHand.Clear();

            DrawCards(cardsToReplace);

            disguardsLeft--;

            RefreshHandUI();
        }
        //I had to change this to an async method for animations. 
        private async void PlayHand_Click(object sender, RoutedEventArgs e)
        {
            //just in case you try and waste a hand. You have to play something to advance the game.
            if (selectedHand.Count == 0)
            {
                return;
            }
                
            //this is for the score, they are doubles as the alrger scores and some other cards
            //can fuck with intagers so Ive started with this. 
            double chipScore = 0, multScore = 0;
            string handPlayed = "";//type of hand played
            //checking for staights. 
            bool isFlush = true;
            int isPair = 0;

            //Assinging the chipps scores to be played. 
            List<TextBlock> cardChipSlots = new List<TextBlock>()
            { ChipScore1,ChipScore2,ChipScore3,ChipScore4,ChipScore5};


            //creating another list of cards that have been played to moove the png up 
            List<Image> cardsPlayedImage = new List<Image>()
            {PlayedImage1 , PlayedImage2 , PlayedImage3 , PlayedImage4 , PlayedImage5};


             //Wanted to sort the hand before hand as it would make it easier to score. 
             selectedHand.Sort();
            //adding the 2 string to the cards.  
            //adding the chip score from each of the hands to this. Adding animations before final scoring 
            for (int i = 0; i < selectedHand.Count; i++)
            {
                chipScore += selectedHand[i].CardChipValue;
            }
            //I removed the async method from the playhand and brought it into the disguards.
            List<Cards> storedSelectedHand = new List<Cards>(selectedHand);

            await AnimatePlayedHand(storedSelectedHand);


            //ive changed this to pass in the main method to call on the other ahand methods. 
            switch (CheckHandTypeMain(storedSelectedHand,isFlush,isPair))
             {   
                    //each of these are hands that have been played. 
                case 0:
                        //High card
                        handPlayed = "high card";
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
                        handPlayed = "Striaght";
                        chipScore += 55;//its harder to get a straight than it is a flush. 
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
                        multScore += 4; ;
                    break;
             }


            //this takes the jokers that are played into effect but for now they are unused. 
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

            int cardsToReplace = selectedHand.Count;
            selectedHand.Clear();

            DrawCards(cardsToReplace);

            handsLeft--;
            RefreshHandUI();

            playersCurrentScore += chipScore * multScore;
            string finalScore = $"{handPlayed}\nScore: {playersCurrentScore}";
            PlayerChipScore.Text = finalScore;

            AnimationCanvas.Children.Clear();
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
            //leaving the players score till the end of the animation to make it more worthwhile. 
            playersCurrentScore += chipScore * multScore;
            //changing the fronend display for the playerscore 
            finalScore = $"{handPlayed}\nScore: {playersCurrentScore}";
            PlayerChipScore.Text = finalScore;
            ResetAllCardAnimations();
            AnimationCanvas.Children.Clear();
            //this clears the cards played section after the hand is played.
            cardsPlayedImage.Clear();
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
            }else
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
                isPair += 2;
                //this was a weird thing to wrap my head around.
                //this is counting 2 cases of 2 pairs which is 2 pairs. 
            }else if (groups.Contains(2))
            {
                isPair++;
            }
                return isPair;
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
        private int CheckHandTypeMain(List<Cards> selectedHand, bool isFlush,int isPair)
        {
            //the point of this method is a main method ot pass into the other hands
            //checking the hand type and pushing the handNumber out so it makes it easier to 
            //visually process the hand type. 
            int handNumber = 0;//this is for high cards.
            //pair
            if (CheckPair(selectedHand, isPair) == 1)
            {
                handNumber = 1;//this is for a pair 
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
            else if (CheckFullHouse(selectedHand))
            {
                handNumber = 6;
            }
            else
            {
                return handNumber;
            }
            return handNumber;
        }
        private async Task PlayCardAnimation(List<Cards> selectedHand, List<Image> cardsPlayedImage, List<TextBlock> cardChipValue)
        {
            for (int i = 0; i < selectedHand.Count; i++)
            {
                cardChipValue[i].Text = $"+{selectedHand[i].CardChipValue}";
                cardsPlayedImage[i].Source = new BitmapImage(
                    new Uri($"pack://application:,,,/Images/Cards/{selectedHand[i].ToString()}")
                );

                await Task.Delay(80);
            }

            await Task.Delay(100);
        }
        private TranslateTransform GetCardTranslate(Button card)
        {
            TransformGroup group = card.RenderTransform as TransformGroup;

            if (group == null || group.IsFrozen)
            {
                group = new TransformGroup();
                group.Children.Add(new ScaleTransform());
                group.Children.Add(new SkewTransform());
                group.Children.Add(new RotateTransform());

                TranslateTransform translate = new TranslateTransform();
                group.Children.Add(translate);

                card.RenderTransform = group;
                return translate;
            }

            TranslateTransform existing = group.Children
                .OfType<TranslateTransform>()
                .FirstOrDefault();

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
        private void AnimateCard(Button card, double toY, double? toX = null)
        {
            var translate = GetCardTranslate(card);

            var animY = new DoubleAnimation
            {
                To = toY,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
            };

            translate.BeginAnimation(TranslateTransform.YProperty, animY);

            if (toX.HasValue)
            {
                var animX = new DoubleAnimation
                {
                    To = toX.Value,
                    Duration = TimeSpan.FromMilliseconds(200),
                    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseOut }
                };

                translate.BeginAnimation(TranslateTransform.XProperty, animX);
            }
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
        private void Card_HoverEnter(object sender, MouseEventArgs e)
        {
            Button card = sender as Button;
            Cards c = card?.Tag as Cards;

            if (c == null)
                return;

            // Don't override selected cards
            if (selectedHand.Contains(c))
                return;

            AnimateCard(card, -10); // small lift
        }

        private void Card_HoverLeave(object sender, MouseEventArgs e)
        {
            Button card = sender as Button;
            Cards c = card?.Tag as Cards;

            if (c == null)
                return;

            // If selected, keep it raised
            if (selectedHand.Contains(c))
                return;

            AnimateCard(card, 0);
        }
        private async Task AnimateCardToPlayArea(Button card, Point targetPosition)
        {
            var translate = GetCardTranslate(card);

            // Get current position relative to window
            Point start = card.TranslatePoint(new Point(0, 0), MainGrid);

            double deltaX = targetPosition.X - start.X;
            double deltaY = targetPosition.Y - (start.Y + 200);

            // X animation
            var animX = new DoubleAnimation
            {
                To = deltaX,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            // Y animation (slight arc effect)
            var animY = new DoubleAnimation
            {
                To = deltaY - 50, // lift upwards slightly
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            translate.BeginAnimation(TranslateTransform.XProperty, animX);
            translate.BeginAnimation(TranslateTransform.YProperty, animY);

            await Task.Delay(300);

            // Small "drop" bounce
            var dropAnim = new DoubleAnimation
            {
                To = deltaY,
                Duration = TimeSpan.FromMilliseconds(120),
                EasingFunction = new BounceEase
                {
                    Bounces = 1,
                    Bounciness = 2,
                    EasingMode = EasingMode.EaseOut
                }
            };

            translate.BeginAnimation(TranslateTransform.YProperty, dropAnim);

            await Task.Delay(120);
        }
        private List<Point> GetPlayTargets()
        {
            List<Image> targets = new List<Image>()
    {
        PlayedImage1, PlayedImage2, PlayedImage3, PlayedImage4, PlayedImage5
    };

            List<Point> positions = new List<Point>();

            foreach (var img in targets)
            {
                Point pos = img.TranslatePoint(new Point(0, 0), MainGrid);
                positions.Add(pos);
            }

            return positions;
        }
        private async Task AnimatePlayedHand(List<Cards> selectedCards)
        {
            var buttons = CardGrid.Children.OfType<Button>().ToList();

            double spacing = 80;
            double lift = -150;

            double centerX = MainGrid.ActualWidth / 2;
            double centerY = MainGrid.ActualHeight / 2;

            double centerOffset = (selectedCards.Count - 1) / 2.0;

            for (int i = 0; i < selectedCards.Count; i++)
            {
                Cards card = selectedCards[i];

                Button original = buttons.FirstOrDefault(b => b.Tag == card);
                if (original == null)
                    continue;

                // 🔥 Move to canvas
                Button clone = MoveToCanvas(original);

                var translate = clone.RenderTransform as TranslateTransform;

                double targetX = centerX + (i - centerOffset) * spacing;
                double targetY = centerY + lift;

                double startX = Canvas.GetLeft(clone);
                double startY = Canvas.GetTop(clone);

                var animX = new DoubleAnimation
                {
                    To = targetX,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
                };

                var animY = new DoubleAnimation
                {
                    To = targetY,
                    Duration = TimeSpan.FromMilliseconds(300),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                clone.BeginAnimation(Canvas.LeftProperty, animX);
                clone.BeginAnimation(Canvas.TopProperty, animY);

                await Task.Delay(80);
            }
        }
        private Button MoveToCanvas(Button card)
        {
            Point pos = card.TranslatePoint(new Point(0, 0), MainGrid);

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
    }
}