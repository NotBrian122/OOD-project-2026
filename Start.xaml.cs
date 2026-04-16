using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Data.Entity;

namespace OOD_project_2026
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public Start()
        {
            InitializeComponent();

        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainGamePlayWindow());
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
            PlayerNameSubmission.Text = "";
        }
    }
}
