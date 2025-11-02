using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Horse : Animal, ICrazyAction
    {
        public Horse(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            PlaySound("Sounds/horse.wav");
            return Properties.Resources.Horse_Sound;
        }

        public string ActCrazy()
        {
            return string.Format(Properties.Resources.Horse_ActCrazy, Name);
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
                
            }
        }
    }
}
