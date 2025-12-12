using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LoomamaaApp.Klassid;
using System.Windows;
using LoomamaaApp.Repositories;
using LoomamaaApp.Logging;
using LoomamaaApp.Database;
using System.Linq;
using System.Collections.Generic;

namespace LoomamaaApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<Animal> repository;
        private readonly ILogger logger;
        private readonly IAnimalDatabaseRepository dbRepository;

        public ObservableCollection<Animal> Animals { get; } = new ObservableCollection<Animal>();
        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        public ObservableCollection<Enclosure<Animal>> Enclosures { get; } = new ObservableCollection<Enclosure<Animal>>();

        private Enclosure<Animal> selectedEnclosure;
        public Enclosure<Animal> SelectedEnclosure
        {
            get => selectedEnclosure;
            set
            {
                if (selectedEnclosure == value) return;
                selectedEnclosure = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private Animal selectedAnimal;
        public Animal SelectedAnimal
        {
            get => selectedAnimal;
            set
            {
                if (selectedAnimal == value) return;
                selectedAnimal = value;
                OnPropertyChanged();
                // Update command states
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string foodInput;
        public string FoodInput
        {
            get => foodInput;
            set
            {
                if (foodInput == value) return;
                foodInput = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand MakeSoundCommand { get; }
        public ICommand FeedCommand { get; }
        public ICommand AddAnimalCommand { get; }
        public ICommand RemoveAnimalCommand { get; }
        public ICommand CrazyActionCommand { get; }
        public ICommand TriggerNightCommand { get; }
        public ICommand OpenLinqCommand { get; }
        public ICommand SaveToDatabaseCommand { get; }
        public ICommand LoadFromDatabaseCommand { get; }

        public MainViewModel(ILogger logger, IRepository<Animal> repository, IAnimalDatabaseRepository dbRepository)
        {
            this.logger = logger;
            this.repository = repository;
            this.dbRepository = dbRepository;

            // create enclosures and subscribe to events
            var north = new Enclosure<Animal>("Põhja");
            var south = new Enclosure<Animal>("Lõuna");

            SubscribeEnclosure(north);
            SubscribeEnclosure(south);

            Enclosures.Add(north);
            Enclosures.Add(south);

            // initial sample data stored in repository and added to an enclosure
            var c = new Cat("Muri", 3);
            var d = new Dog("Sharik", 5);

            repository.Add(c);
            repository.Add(d);

            north.AddAnimal(c);
            north.AddAnimal(d);

            // populate observable collection from repository
            foreach (var a in repository.GetAll())
                Animals.Add(a);

            MakeSoundCommand = new RelayCommand(_ => MakeSound(), _ => SelectedAnimal != null);
            FeedCommand = new RelayCommand(_ => Feed(), _ => SelectedAnimal != null && !string.IsNullOrWhiteSpace(FoodInput));
            AddAnimalCommand = new RelayCommand(_ => AddAnimal());
            RemoveAnimalCommand = new RelayCommand(_ => RemoveAnimal(), _ => SelectedAnimal != null);
            CrazyActionCommand = new RelayCommand(_ => CrazyAction(), _ => SelectedAnimal != null);
            TriggerNightCommand = new RelayCommand(_ => TriggerNight(), _ => SelectedEnclosure != null || SelectedAnimal != null);
            OpenLinqCommand = new RelayCommand(_ => OpenLinq());
            SaveToDatabaseCommand = new RelayCommand(_ => SaveToDatabase());
            LoadFromDatabaseCommand = new RelayCommand(_ => LoadFromDatabase());

            // Log application start
            logger.Log("Application started");
        }

        private void SubscribeEnclosure(Enclosure<Animal> enclosure)
        {
            enclosure.AnimalAdded += Enclosure_AnimalAdded;
            enclosure.FoodDropped += Enclosure_FoodDropped;
            enclosure.NightOccurred += Enclosure_NightOccurred;
        }

        private void UnsubscribeEnclosure(Enclosure<Animal> enclosure)
        {
            enclosure.AnimalAdded -= Enclosure_AnimalAdded;
            enclosure.FoodDropped -= Enclosure_FoodDropped;
            enclosure.NightOccurred -= Enclosure_NightOccurred;
        }

        private void Enclosure_AnimalAdded(object sender, AnimalEventArgs e)
        {
            var enc = sender as Enclosure<Animal>;
            var message = $"[{enc?.Name}] Animal added: {e.Animal.Name} ({e.Animal.TypeName})";
            AddLog(message);
            logger.Log(message);
        }

        private void Enclosure_FoodDropped(object sender, FoodDroppedEventArgs e)
        {
            var enc = sender as Enclosure<Animal>;
            var message = $"[{enc?.Name}] Food dropped: {e.Food}";
            AddLog(message);
            logger.Log(message);
        }

        private void Enclosure_NightOccurred(object sender, NightEventArgs e)
        {
            var enc = sender as Enclosure<Animal>;
            var message = $"[{enc?.Name}] Night event: {e.Description}";
            AddLog(message);
            logger.Log(message);
        }

        private void AddLog(string message)
        {
            Logs.Insert(0, message);
        }

        private void MakeSound()
        {
            if (SelectedAnimal == null) return;
            string sound = SelectedAnimal.MakeSound();
            var message = string.Format(Properties.Resources.LogSaidFormat, SelectedAnimal.Name, sound);
            AddLog(message);
            logger.Log(message);
        }

        private void Feed()
        {
            if (SelectedAnimal == null) return;
            string food = FoodInput?.Trim();
            if (string.IsNullOrEmpty(food)) return;
            var message = string.Format(Properties.Resources.LogAteFormat, SelectedAnimal.Name, food);
            AddLog(message);
            logger.Log(message);
            FoodInput = string.Empty;
        }

        private void AddAnimal()
        {
            // pass enclosures so the user can pick where to place the new animal
            var addWindow = new AddAnimalWindow(Enclosures, SelectedEnclosure);
            // try to set owner if available
            if (Application.Current?.MainWindow != null)
                addWindow.Owner = Application.Current.MainWindow;

            if (addWindow.ShowDialog() == true && addWindow.NewAnimal != null)
            {
                // add to repository
                repository.Add(addWindow.NewAnimal);
                Animals.Insert(0, addWindow.NewAnimal);

                // if the user picked an enclosure in the dialog, add to it
                if (addWindow.ChosenEnclosure != null)
                {
                    addWindow.ChosenEnclosure.AddAnimal(addWindow.NewAnimal);
                }

                var message = $"Added new animal: {addWindow.NewAnimal.Name} ({addWindow.NewAnimal.TypeName})";
                logger.Log(message);
            }
        }

        private void RemoveAnimal()
        {
            if (SelectedAnimal == null) return;

            var animalName = SelectedAnimal.Name;
            var animalType = SelectedAnimal.TypeName;

            // remove from repository
            repository.Remove(SelectedAnimal);

            // remove from any enclosure that contains it
            foreach (var enc in Enclosures)
            {
                if (enc.Animals.Contains(SelectedAnimal))
                    enc.RemoveAnimal(SelectedAnimal);
            }

            Animals.Remove(SelectedAnimal);
            SelectedAnimal = null;

            var message = $"Removed animal: {animalName} ({animalType})";
            AddLog(message);
            logger.Log(message);
        }

        private void CrazyAction()
        {
            if (SelectedAnimal == null) return;
            string message;
            if (SelectedAnimal is ICrazyAction crazy)
            {
                message = crazy.ActCrazy();
            }
            else
            {
                message = string.Format(Properties.Resources.NoCrazyActionFormat, SelectedAnimal.Name);
            }
            AddLog(message);
            logger.Log(message);
        }

        private void TriggerNight()
        {
            Enclosure<Animal> target = SelectedEnclosure;
            if (target == null && SelectedAnimal != null)
            {
                target = Enclosures.FirstOrDefault(e => e.Animals.Contains(SelectedAnimal));
            }

            if (target == null) return;

            target.TriggerNight("A mysterious night event occurred.");
        }

        private void OpenLinq()
        {
            var win = new LoomamaaApp.LinqWindow(Animals, Enclosures);
            if (Application.Current?.MainWindow != null)
                win.Owner = Application.Current.MainWindow;
            win.ShowDialog();
        }

        private void SaveToDatabase()
        {
            try
            {
                // Clear existing data to prevent duplicates
                dbRepository.ClearDatabase();

                // Save all enclosures with their animals
                foreach (var enclosure in Enclosures)
                {
                    dbRepository.SaveEnclosure(enclosure);
                }

                var message = $"Saved {Enclosures.Count} enclosures to database";
                AddLog(message);
                logger.Log(message);
                MessageBox.Show("Data saved to database successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                var message = $"Error saving to database: {ex.Message}";
                AddLog(message);
                logger.Log(message);
                MessageBox.Show($"Failed to save to database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadFromDatabase()
        {
            try
            {
                // Load enclosures from database
                var loadedEnclosures = dbRepository.LoadEnclosures();
                
                if (loadedEnclosures != null && loadedEnclosures.Any())
                {
                    // Unsubscribe from current enclosures to prevent memory leaks
                    foreach (var enc in Enclosures)
                    {
                        UnsubscribeEnclosure(enc);
                    }

                    // Clear current data
                    Enclosures.Clear();
                    Animals.Clear();

                    // Add loaded enclosures
                    foreach (var enc in loadedEnclosures)
                    {
                        SubscribeEnclosure(enc);
                        Enclosures.Add(enc);

                        // Add animals to the main collection
                        foreach (var animal in enc.Animals)
                        {
                            Animals.Add(animal);
                            repository.Add(animal);
                        }
                    }

                    var message = $"Loaded {Enclosures.Count} enclosures from database";
                    AddLog(message);
                    logger.Log(message);
                    MessageBox.Show("Data loaded from database successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No data found in database.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                var message = $"Error loading from database: {ex.Message}";
                AddLog(message);
                logger.Log(message);
                MessageBox.Show($"Failed to load from database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
