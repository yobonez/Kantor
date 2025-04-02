using KantorWPF.ViewModel;
using System.Windows;

namespace KantorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainWindowViewModel vmMainWindowViewModel = new();
            DataContext = vmMainWindowViewModel;
            InitializeComponent();
        }
    }
}