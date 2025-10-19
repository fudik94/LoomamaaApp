using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Sheep : Animal, ICrazyAction
    {
        public Sheep(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            SoundPlayer player = new SoundPlayer("Sounds/sheep.wav");
            player.Play();
            return "Baa!";
        }

        public string ActCrazy() => $"{Name} jooksis ringi nagu hull lammas!";
    }
}
