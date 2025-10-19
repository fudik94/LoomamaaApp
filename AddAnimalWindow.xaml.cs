using LoomamaaApp.Klassid;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LoomamaaApp
{
    public partial class AddAnimalWindow : Window
    {
        // Property to store the new animal created in this window
        public Animal NewAnimal { get; private set; }

        public AddAnimalWindow()
        {
            InitializeComponent(); // Initialize the UI components
        }

        // This method is called when the "Add" button is clicked
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the name entered by the user
            string name = NameTextBox.Text.Trim();

            // Try to parse the age; show message if invalid
            if (!int.TryParse(AgeTextBox.Text.Trim(), out int age))
            {
                MessageBox.Show("Введите корректный возраст!"); // Invalid age
                return;
            }

            // Check if a type of animal is selected
            if (TypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип животного!"); // No type selected
                return;
            }

            // Get the selected animal type from the ComboBox
            string type = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();

            // Create the corresponding animal object based on type
            switch (type)
            {
                case "Cat": NewAnimal = new Cat(name, age); break;
                case "Dog": NewAnimal = new Dog(name, age); break;
                case "Monkey": NewAnimal = new Monkey(name, age); break;
                case "Sheep": NewAnimal = new Sheep(name, age); break;
                case "Pig": NewAnimal = new Pig(name, age); break;
            }

            // Close the window and set DialogResult to true so MainWindow knows an animal was created
            this.DialogResult = true;
            this.Close();
        }
    }
}
