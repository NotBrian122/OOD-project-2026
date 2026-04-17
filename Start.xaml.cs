using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Data.Entity;
using System.Linq;

namespace OOD_project_2026
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        LeaderBoard db = new LeaderBoard();
        string playerName ="";

        public Start()
        {
            InitializeComponent();

        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            //Ive had to learn how to pass in new variables into the gameplay window. 
            NavigationService.Navigate(new MainGamePlayWindow(playerName));
        }

        private void SumbitName(object sender, RoutedEventArgs e)
        {
            if (PlayerNameSubmission.Text == "")
            {
                PlayerNameSubmission.Text = "You must input a name to start the game.";
                return;
            }
            else
            {
                playerName = PlayerNameSubmission.Text;
                //allowing you to input a name.
                StartGame.IsHitTestVisible = true;
                StartGame.Visibility = Visibility.Visible;
                //disabling the name. 
                SubmitContent.IsHitTestVisible = false;
                SubmitContent.Visibility = Visibility.Collapsed;
                PlayerNameSubmission.IsHitTestVisible = false;
                PlayerNameSubmission.Visibility = Visibility.Collapsed;
            }
        }

        private void PlayerNameSubmission_GotFocus(object sender, RoutedEventArgs e)
        {
            //just removing the text  and mamking it easier to input a name.
            PlayerNameSubmission.Text = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //just to remove this from the main window. 
            LoadLeaderboard();
     
        }
        private void LoadLeaderboard()
        {
            using (var db = new LeaderBoard())
            {
                //loaing the leaderbaord by ordring by the highscore then by rounds lasted. 
                var data = db.HighScoreData
                    .OrderByDescending(x => x.HighScore)
                    .ThenByDescending(x => x.RoundsLasted)
                    .ToList()
                    .Select((x, index) => new HighScoreData
                    {
                        //logging new highscore data. 
                        Rank = index + 1,//index +1 to make it easier on ranking teh system. 
                        PlayerName = x.PlayerName,
                        HighScore = x.HighScore,
                        RoundsLasted = x.RoundsLasted,
                        Date = x.Date
                    })
                    .ToList();

                PlayerNameList.ItemsSource = data;
            }
        }
    }
}
