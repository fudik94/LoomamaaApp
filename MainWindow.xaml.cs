using System.Windows;
using LoomamaaApp.ViewModels;

namespace LoomamaaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}
