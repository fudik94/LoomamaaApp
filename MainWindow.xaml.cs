using System.Collections.ObjectModel;
using System.Windows;
using LoomamaaApp.Klassid;

namespace LoomamaaApp
{
    public partial class MainWindow : Window
    {
        // ObservableCollection allows the ListBox to automatically update when items are added or removed
        private ObservableCollection<Animal> animals = new ObservableCollection<Animal>();

        public MainWindow()
        {
            InitializeComponent();
            // Bind the ListBox to the animals collection
            AnimalsListBox.ItemsSource = animals;

            // Add some initial animals to the list
            animals.Add(new Cat("Muri", 3));
            animals.Add(new Dog("Sharik", 5));
        }

        // Update the detail panel when a different animal is selected
        private void AnimalsListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                NameTextBlock.Text = selectedAnimal.Name; // display the animal's name
                AgeTextBlock.Text = selectedAnimal.Age.ToString(); // display the animal's age
            }
            else
            {
                NameTextBlock.Text = "";
                AgeTextBlock.Text = "";
            }
        }

        // Make the selected animal sound and log it
        private void MakeSoundButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                string sound = selectedAnimal.MakeSound(); // call the animal's MakeSound method
                LogListBox.Items.Add($"{selectedAnimal.Name} ütles: {sound}"); // add result to log
            }
        }

        // Feed the selected animal with the text from the TextBox and log the action
        private void FeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                string food = FoodTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(food))
                {
                    LogListBox.Items.Add($"{selectedAnimal.Name} sõi {food}"); // add feeding info to log
                    FoodTextBox.Clear(); // clear the input after feeding
                }
            }
        }

        // Open the AddAnimalWindow to create a new animal
        private void AddAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            AddAnimalWindow addWindow = new AddAnimalWindow();
            if (addWindow.ShowDialog() == true && addWindow.NewAnimal != null)
            {
                animals.Add(addWindow.NewAnimal); // add the new animal to the collection
            }
        }

        // Remove the selected animal from the list
        private void RemoveAnimalButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                animals.Remove(selectedAnimal);
            }
        }

        // Perform the "crazy action" of the selected animal if it implements ICrazyAction
        private void CrazyActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (AnimalsListBox.SelectedItem is ICrazyAction crazyAnimal)
            {
                LogListBox.Items.Add(crazyAnimal.ActCrazy()); // add crazy action to log
            }
            else if (AnimalsListBox.SelectedItem is Animal selectedAnimal)
            {
                LogListBox.Items.Add($"{selectedAnimal.Name} ei tee crazy action."); // if no crazy action
            }
        }
    }
}
