using LoomamaaApp.Klassid;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LoomamaaApp
{
    public partial class AddAnimalWindow : Window
    {
        // Stores the newly created animal from this window
        public Animal NewAnimal { get; private set; }

        public AddAnimalWindow()
        {
            InitializeComponent(); // Initialize the ui
        }

        // Called when the add button is clic
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Get and validate name
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a valid name!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get and validate age
            if (!int.TryParse(AgeTextBox.Text.Trim(), out int age))
            {
                MessageBox.Show("Please enter a valid number for age!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (age <= 0 || age > 100)
            {
                MessageBox.Show("Age must be between 1 and 100!", "Invalid Age", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Check that animal type is selected
            if (TypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select an animal type!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get selected type
            string type = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();

            // Create the corresponding animal object
            switch (type)
            {
                case "Cat":
                    NewAnimal = new Cat(name, age);
                    break;
                case "Dog":
                    NewAnimal = new Dog(name, age);
                    break;
                case "Monkey":
                    NewAnimal = new Monkey(name, age);
                    break;
                case "Sheep":
                    NewAnimal = new Sheep(name, age);
                    break;
                case "Pig":
                    NewAnimal = new Pig(name, age);
                    break;
                case "Horse":
                    NewAnimal = new Horse(name, age);
                    break;
                case "Duck":
                    NewAnimal = new Duck(name, age);
                    break;
                case "Bear":
                    NewAnimal = new Bear(name, age);
                    break;
                default:
                    MessageBox.Show("Unknown animal type!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
            }

            // If animal successfully created, close the window
            this.DialogResult = true;
            this.Close();
        }
    }
}
