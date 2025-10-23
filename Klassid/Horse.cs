using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Horse : Animal, ICrazyAction
    {
        public Horse(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            PlaySound("Sounds/horse.wav");
            return "Neigh!";
        }

        public string ActCrazy()
        {
            return $"{Name} started galloping in circles! ";
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
                // ignore if sound file is missing
            }
        }
    }
}
