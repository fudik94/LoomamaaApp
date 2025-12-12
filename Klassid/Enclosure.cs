using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace LoomamaaApp.Klassid
{
    public class AnimalEventArgs : EventArgs
    {
        public Animal Animal { get; }
        public AnimalEventArgs(Animal animal) { Animal = animal; }
    }

    public class AnimalJoinedEventArgs : EventArgs
    {
        public Animal NewAnimal { get; }
        public Animal ExistingAnimal { get; }
        public AnimalJoinedEventArgs(Animal newAnimal, Animal existingAnimal)
        {
            NewAnimal = newAnimal;
            ExistingAnimal = existingAnimal;
        }
    }

    public class FoodDroppedEventArgs : EventArgs
    {
        public string Food { get; }
        public FoodDroppedEventArgs(string food) { Food = food; }
    }

    public class FoodEatingProgressEventArgs : EventArgs
    {
        public Animal Animal { get; }
        public int Progress { get; }
        public string Food { get; }
        public FoodEatingProgressEventArgs(Animal animal, string food, int progress)
        {
            Animal = animal; Food = food; Progress = progress;
        }
    }

    public class NightEventArgs : EventArgs
    {
        public string Description { get; }
        public NightEventArgs(string description) { Description = description; }
    }

    public class Enclosure<T> where T : Animal
    {
        public string Name { get; set; }
        public ObservableCollection<T> Animals { get; } = new ObservableCollection<T>();

        // Events
        public event EventHandler<AnimalEventArgs> AnimalAdded;
        public event EventHandler<AnimalJoinedEventArgs> AnimalJoinedInSameEnclosure;
        public event EventHandler<FoodDroppedEventArgs> FoodDropped;
        public event EventHandler<FoodEatingProgressEventArgs> FoodEatingProgress;
        public event EventHandler<NightEventArgs> NightOccurred;

        private Timer nightTimer;

        public Enclosure() { }
        public Enclosure(string name) { Name = name; }

        public void AddAnimal(T animal)
        {
            if (animal == null) throw new ArgumentNullException(nameof(animal));

            // For each existing animal, raise Joined event to indicate interaction
            foreach (var existing in Animals)
            {
                OnAnimalJoined(animal, existing);
            }

            Animals.Add(animal);
            OnAnimalAdded(animal);
        }

        public bool RemoveAnimal(T animal)
        {
            if (animal == null) return false;
            return Animals.Remove(animal);
        }

        public void DropFood(string food)
        {
            if (food == null) throw new ArgumentNullException(nameof(food));
            OnFoodDropped(food);

            // simulate eating for each animal asynchronously, with different durations
            foreach (var a in Animals)
            {
                SimulateEatingAsync(a, food);
            }
        }

        public void TriggerNight(string description)
        {
            OnNightOccurred(description);
        }

        public void StartNightCycle(int intervalSeconds = 10)
        {
            StopNightCycle();
            nightTimer = new Timer(_ => OnNightOccurred("Night cycle tick"), null, intervalSeconds * 1000, intervalSeconds * 1000);
        }

        public void StopNightCycle()
        {
            try { nightTimer?.Dispose(); } catch { }
            nightTimer = null;
        }

        private async void SimulateEatingAsync(T animal, string food)
        {
            try
            {
                var rnd = new Random(Guid.NewGuid().GetHashCode());
                int totalMs = rnd.Next(1500, 6000);
                int steps = 10;
                int stepMs = Math.Max(100, totalMs / steps);

                for (int i = 1; i <= steps; i++)
                {
                    await Task.Delay(stepMs).ConfigureAwait(false);
                    int progress = i * 100 / steps;
                    OnFoodEatingProgress(animal, food, progress);
                }
            }
            catch
            {
                // ignore
            }
        }

        protected virtual void OnAnimalAdded(Animal animal)
        {
            AnimalAdded?.Invoke(this, new AnimalEventArgs(animal));
        }

        protected virtual void OnAnimalJoined(Animal newAnimal, Animal existing)
        {
            AnimalJoinedInSameEnclosure?.Invoke(this, new AnimalJoinedEventArgs(newAnimal, existing));
        }

        protected virtual void OnFoodDropped(string food)
        {
            FoodDropped?.Invoke(this, new FoodDroppedEventArgs(food));
        }

        protected virtual void OnFoodEatingProgress(Animal animal, string food, int progress)
        {
            FoodEatingProgress?.Invoke(this, new FoodEatingProgressEventArgs(animal, food, progress));
        }

        protected virtual void OnNightOccurred(string description)
        {
            NightOccurred?.Invoke(this, new NightEventArgs(description));
        }
    }
}
