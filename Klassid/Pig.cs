using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Pig : Animal, ICrazyAction
    {
        public Pig(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            SoundPlayer player = new SoundPlayer("Sounds/pig.wav");
            player.Play();
            return Properties.Resources.Pig_Sound;
        }

        public string ActCrazy() => string.Format(Properties.Resources.Pig_ActCrazy, Name);
    }
}
