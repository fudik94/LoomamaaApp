using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Bear : Animal, ICrazyAction
    {
        public Bear(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            PlaySound("Sounds/bear.wav");
            return Properties.Resources.Bear_Sound;
        }

        public string ActCrazy()
        {
            return string.Format(Properties.Resources.Bear_ActCrazy, Name);
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
