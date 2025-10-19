namespace LoomamaaApp.Klassid
{
    public abstract class Animal
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Animal(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public virtual string Describe() => $"{Name}, {Age} aastat vana";
        public abstract string MakeSound();

        public override string ToString()
        {
            return $"{Name} {this.GetType().Name}";
        }
    }
}
