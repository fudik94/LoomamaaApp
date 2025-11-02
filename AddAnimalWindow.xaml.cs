using LoomamaaApp.Klassid;
using System;
using System.Windows;
using System.Windows.Controls;

namespace LoomamaaApp
{
    public partial class AddAnimalWindow : Window
    {
        // New animal created in this window
        public Animal NewAnimal { get; private set; }

        public AddAnimalWindow()
        {
            InitializeComponent();
        }

        // Called when "Add" button is clicked
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(Properties.Resources.PleaseEnterValidName,
                                Properties.Resources.InputErrorTitle,
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(AgeTextBox.Text.Trim(), out int age))
            {
                MessageBox.Show(Properties.Resources.PleaseEnterValidNumberAge,
                                Properties.Resources.InputErrorTitle,
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (age <= 0 || age > 100)
            {
                MessageBox.Show(Properties.Resources.AgeRangeError,
                                Properties.Resources.InputErrorTitle,
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (TypeComboBox.SelectedItem == null)
            {
                MessageBox.Show(Properties.Resources.PleaseSelectAnimalType,
                                Properties.Resources.InputErrorTitle,
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            // Get animal type from ComboBox Tag
            var tagObj = ((ComboBoxItem)TypeComboBox.SelectedItem).Tag;
            if (!(tagObj is AnimalType type))
            {
                MessageBox.Show(Properties.Resources.UnknownAnimalType,
                                Properties.Resources.ErrorTitle,
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            // Create animal based on type
            switch (type)
            {
                case AnimalType.Cat: NewAnimal = new Cat(name, age); break;
                case AnimalType.Dog: NewAnimal = new Dog(name, age); break;
                case AnimalType.Monkey: NewAnimal = new Monkey(name, age); break;
                case AnimalType.Sheep: NewAnimal = new Sheep(name, age); break;
                case AnimalType.Pig: NewAnimal = new Pig(name, age); break;
                case AnimalType.Horse: NewAnimal = new Horse(name, age); break;
                case AnimalType.Duck: NewAnimal = new Duck(name, age); break;
                case AnimalType.Bear: NewAnimal = new Bear(name, age); break;
                default:
                    MessageBox.Show(Properties.Resources.UnknownAnimalType,
                                    Properties.Resources.ErrorTitle,
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
            }

            this.DialogResult = true;
            this.Close();
        }
    }
}
