using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Dog : Animal, ICrazyAction
    {
        public Dog(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            SoundPlayer player = new SoundPlayer("Sounds/dog.wav");
            player.Play();
            return "Woof!";
        }

        public string ActCrazy() => $"{Name} haugub 5 korda järjest: Woof! Woof! Woof! Woof! Woof!";
    }
}
