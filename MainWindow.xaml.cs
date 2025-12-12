using System.Windows;
using LoomamaaApp.ViewModels;

namespace LoomamaaApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Get MainViewModel from DI container
            var logger = ServiceLocator.Instance.Resolve<Logging.ILogger>();
            var repository = ServiceLocator.Instance.Resolve<Repositories.IRepository<Klassid.Animal>>();
            var dbRepository = ServiceLocator.Instance.Resolve<Database.IAnimalDatabaseRepository>();
            
            this.DataContext = new MainViewModel(logger, repository, dbRepository);
        }

        private void EnclosuresListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
