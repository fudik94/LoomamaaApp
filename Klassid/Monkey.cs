using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Monkey : Animal, ICrazyAction
    {
        public Monkey(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            SoundPlayer player = new SoundPlayer("Sounds/monkey.wav");
            player.Play();
            return "Oo-oo-aa-aa!";
        }

        public string ActCrazy() => $"{Name} vahetas kahe looma nimed omavahel!";
    }
}
