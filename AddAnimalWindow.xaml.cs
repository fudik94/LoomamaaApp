using LoomamaaApp.Klassid;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

namespace LoomamaaApp
{
    public partial class AddAnimalWindow : Window
    {
        // New animal created in this window
        public Animal NewAnimal { get; private set; }

        // Enclosure chosen by the user (set after pressing Add)
        public Enclosure<Animal> ChosenEnclosure { get; private set; }

        public AddAnimalWindow()
        {
            InitializeComponent();
        }

        // New ctor to populate enclosures list and optionally preselect one
        public AddAnimalWindow(IEnumerable<Enclosure<Animal>> enclosures, Enclosure<Animal> preselected = null)
            : this()
        {
            if (enclosures != null)
            {
                var combo = this.FindName("EnclosureComboBox") as ComboBox;
                if (combo != null)
                {
                    combo.ItemsSource = enclosures;
                    if (preselected != null)
                        combo.SelectedItem = preselected;
                }
            }
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

            // Capture chosen enclosure (may be null)
            var combo2 = this.FindName("EnclosureComboBox") as ComboBox;
            ChosenEnclosure = combo2?.SelectedItem as Enclosure<Animal>;

            this.DialogResult = true;
            this.Close();
        }
    }
}
