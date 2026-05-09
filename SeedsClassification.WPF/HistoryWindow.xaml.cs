using System.Windows;
using SeedsClassification.WPF.ViewModels;

namespace SeedsClassification.WPF.Views
{
    public partial class HistoriqueWindow : Window
    {
        public HistoriqueWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.ChargerHistorique();
        }
    }
}