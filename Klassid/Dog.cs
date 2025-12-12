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
            return Properties.Resources.Dog_Sound;
        }

        public string ActCrazy() => string.Format(Properties.Resources.Dog_ActCrazy, Name);
    }
}
