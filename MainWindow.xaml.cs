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
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        List<Cards> PlayerDeck = new List<Cards>();
        public MainWindow()
        {
            InitializeComponent();
            //creating new list to create deck to add or subtrack cards from.
            
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            GenerateDeck();
            //calling the generating deck metho as to easily create something on click 
        }
        public void GenerateDeck()
        {
            //creating a for loop to generate the deck of all cards needed
            for (int i = 0; i < 4; i++)//every suit for every deck
            {
                //counts each card number
                for (int j = 1; j < 14; j++)
                {
                    switch (i)
                    {
                        //public Cards(string CardName, int CardChipValue, string SuitName, string Effect)
                        case 0:
                            Cards cardH = new Cards($"{j}",j,"Hearts", "None");//so a suit then a value of 1 => 14
                            PlayerDeck.Add(cardH);

                            break;
                        case 1:
                            Cards cardD = new Cards($"{j}",j, "Diamonds","None");
                           PlayerDeck.Add(cardD);
                            break;
                        case 2:
                            Cards cardS = new Cards($"{j}",j, "Spades","None");
                           PlayerDeck.Add(cardS);
                            break;
                        case 3:
                            Cards cardC = new Cards($"{j}",j, "Clubs","None");
                            PlayerDeck.Add(cardC);
                            break;
                    }
                }
                
            }

        }
    }
}
