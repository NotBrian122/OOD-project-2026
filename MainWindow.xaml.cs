using System.Windows;

namespace OOD_project_2026
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            //creating new list to create deck to add or subtrack cards from
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Main.Content = new Start();

        }

    }
}
