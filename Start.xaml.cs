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
          if(PlayerNameSubmission.Text == "")
            {
                PlayerNameSubmission.Text = "You must input a name to start the game.";
            }
            else
            {
                //allowing you to input a name.
                StartGame.IsHitTestVisible = true;
                //disabling the name. 
                SumbitName.IsHitTestVisible = false;
            }
        }

        private void PlayerNameSubmission_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PlayerNameSubmission.Text = "";
        }
    }
}
