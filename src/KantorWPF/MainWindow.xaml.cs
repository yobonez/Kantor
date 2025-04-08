using KantorWPF.ViewModel;
using System.Windows;

namespace KantorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel vmMainWindowViewModel = new();
        public MainWindow()
        {
            DataContext = vmMainWindowViewModel;
            InitializeComponent();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == string.Empty)
            {
                searchTextBox.Text = "Szukaj...";
            }
        }

        private async void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            await vmMainWindowViewModel.SearchData();
        }

        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == "Szukaj...")
            {
                searchTextBox.Text = string.Empty;
            }
        }
    }
}