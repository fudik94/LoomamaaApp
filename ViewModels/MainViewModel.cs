using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LoomamaaApp.Klassid;
using System.Windows;
using LoomamaaApp.Repositories;
using System.Linq;
using System.Collections.Generic;

namespace LoomamaaApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IRepository<Animal> repository;

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

        public MainViewModel()
        {
            repository = new AnimalRepository();

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
        }

        private void SubscribeEnclosure(Enclosure<Animal> enclosure)
        {
            enclosure.AnimalAdded += Enclosure_AnimalAdded;
            enclosure.FoodDropped += Enclosure_FoodDropped;
            enclosure.NightOccurred += Enclosure_NightOccurred;
        }

        private void Enclosure_AnimalAdded(object sender, AnimalEventArgs e)
        {
            var enc = sender as Enclosure<Animal>;
            AddLog($"[{enc?.Name}] Animal added: {e.Animal.Name} ({e.Animal.TypeName})");
        }

        private void Enclosure_FoodDropped(object sender, FoodDroppedEventArgs e)
        {
            var enc = sender as Enclosure<Animal>;
            AddLog($"[{enc?.Name}] Food dropped: {e.Food}");
        }

        private void Enclosure_NightOccurred(object sender, NightEventArgs e)
        {
            var enc = sender as Enclosure<Animal>;
            AddLog($"[{enc?.Name}] Night event: {e.Description}");
        }

        private void AddLog(string message)
        {
            Logs.Insert(0, message);
        }

        private void MakeSound()
        {
            if (SelectedAnimal == null) return;
            string sound = SelectedAnimal.MakeSound();
            AddLog(string.Format(Properties.Resources.LogSaidFormat, SelectedAnimal.Name, sound));
        }

        private void Feed()
        {
            if (SelectedAnimal == null) return;
            string food = FoodInput?.Trim();
            if (string.IsNullOrEmpty(food)) return;
            AddLog(string.Format(Properties.Resources.LogAteFormat, SelectedAnimal.Name, food));
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
            }
        }

        private void RemoveAnimal()
        {
            if (SelectedAnimal == null) return;

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
        }

        private void CrazyAction()
        {
            if (SelectedAnimal == null) return;
            if (SelectedAnimal is ICrazyAction crazy)
            {
                AddLog(crazy.ActCrazy());
            }
            else
            {
                AddLog(string.Format(Properties.Resources.NoCrazyActionFormat, SelectedAnimal.Name));
            }
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
