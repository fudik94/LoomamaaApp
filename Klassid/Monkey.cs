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
            return Properties.Resources.Monkey_Sound;
        }

        public string ActCrazy() => string.Format(Properties.Resources.Monkey_ActCrazy, Name);
    }
}
