using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LoomamaaApp.Klassid
{
    // Base class for all animals with property change notifications
    public abstract class Animal : INotifyPropertyChanged
    {
        private string name;
        private int age;

        // EF Core properties
        public int Id { get; set; }
        public int? EnclosureId { get; set; }
        public System.DateTime CreatedDate { get; set; } = System.DateTime.Now;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        // Animal name
        public string Name
        {
            get => name;
            set
            {
                if (name == value) return;
                name = value;
                OnPropertyChanged();
            }
        }

        // Animal age
        public int Age
        {
            get => age;
            set
            {
                if (age == value) return;
                age = value;
                OnPropertyChanged();
            }
        }

        protected Animal(string name, int age)
        {
            this.name = name;
            this.age = age;
        }

        // Description of the animal
        public virtual string Describe() => $"{Name}, {Age} years old";

        // Each animal must implement its sound
        public abstract string MakeSound();

        // Name of the animal type (Cat, Dog, etc.)
        public string TypeName => this.GetType().Name;

        // For debugging or simple display
        public override string ToString() => $"{Name} {TypeName}";
    }
}
