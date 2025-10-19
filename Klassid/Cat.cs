using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Cat : Animal, ICrazyAction
    {
        public Cat(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            
            

            SoundPlayer player = new SoundPlayer("Sounds/cat.wav");
            player.Play();
            return "Meow!";
        }

        public string ActCrazy() => $"{Name} varastas köögist juustu!";
    }
}
