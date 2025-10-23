using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Duck : Animal, ICrazyAction
    {
        public Duck(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            PlaySound("Sounds/duck.wav");
            return "Quack! Quack!";
        }

        public string ActCrazy()
        {
            return $"{Name} flapped its wings and splashed water everywhere! ";
        }

        private void PlaySound(string filePath)
        {
            try
            {
                SoundPlayer player = new SoundPlayer(filePath);
                player.Play();
            }
            catch
            {
                // ignore missing sound file
            }
        }
    }
}
