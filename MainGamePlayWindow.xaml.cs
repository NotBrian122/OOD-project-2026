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



    public partial class MainGamePlayWindow : Page
    {
        public MainGamePlayWindow()
        {
            InitializeComponent();
        }

        private void CardGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //so this is the section in which the random cards are given from the deck.
            //The grid is loaded. 


        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //This is where I want to load my cards. 
        }
    }
}
