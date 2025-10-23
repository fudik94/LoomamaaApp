using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Bear : Animal, ICrazyAction
    {
        public Bear(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            PlaySound("Sounds/bear.wav");
            return "Grrrrr!";
        }

        public string ActCrazy()
        {
            return $"{Name} rolled on the ground and scared everyone! ";
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
