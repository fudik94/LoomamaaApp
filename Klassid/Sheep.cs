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
            return Properties.Resources.Sheep_Sound;
        }

        public string ActCrazy() => string.Format(Properties.Resources.Sheep_ActCrazy, Name);
    }
}
