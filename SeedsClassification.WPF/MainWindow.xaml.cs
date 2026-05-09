using System.Windows;
using SeedsClassification.WPF.ViewModels;
using SeedsClassification.WPF.Views;

namespace SeedsClassification.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = new MainViewModel();
            DataContext = vm;
        }

        private void OpenHistory(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel)DataContext;
            new HistoriqueWindow(vm).ShowDialog();
        }
    }
}