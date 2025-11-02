using System.Collections.ObjectModel;
using System.Windows;
using LoomamaaApp.Klassid;

namespace LoomamaaApp
{
    public partial class MainWindow : Window
    {
        // Collection of animals
        private ObservableCollection<Animal> animals = new ObservableCollection<Animal>();

        // Log entries, newest first
        private ObservableCollection<string> logs = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            AnimalsListBox.ItemsSource = animals;
            LogListBox.ItemsSource = logs;

            // Initial animals
            animals.Add(new Cat("Muri", 3));
            animals.Add(new Dog("Sharik", 5));
        }

        // Add log entry at the top
        private void AddLog(string message)
        {
            logs.Insert(0, message);
            if (LogListBox.Items.Count > 0)
                LogListBox.ScrollIntoView(LogListBox.Items[0]);
        }

        private void MakeSoundButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                string sound = selectedAnimal.MakeSound();
                AddLog(string.Format(Properties.Resources.LogSaidFormat, selectedAnimal.Name, sound));
            }
        }

        private void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                string food = FoodTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(food))
                {
                    AddLog(string.Format(Properties.Resources.LogAteFormat, selectedAnimal.Name, food));
                    FoodTextBox.Clear();
                }
            }
        }

        private void AddAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            AddAnimalWindow addWindow = new AddAnimalWindow();
            if (addWindow.ShowDialog() == true && addWindow.NewAnimal != null)
            {
                animals.Insert(0, addWindow.NewAnimal); // add new animal at top
            }
        }

        private void RemoveAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                animals.Remove(selectedAnimal);
            }
        }

        private void CrazyActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is ICrazyAction crazyAnimal)
            {
                AddLog(crazyAnimal.ActCrazy());
            }
            else if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                AddLog(string.Format(Properties.Resources.NoCrazyActionFormat, selectedAnimal.Name));
            }
        }

        private void AnimalsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
        }
    }
}
