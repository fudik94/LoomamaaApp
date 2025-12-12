using System.Media;

namespace LoomamaaApp.Klassid
{
    public class Duck : Animal, ICrazyAction
    {
        public Duck(string name, int age) : base(name, age) { }

        public override string MakeSound()
        {
            PlaySound("Sounds/duck.wav");
            return Properties.Resources.Duck_Sound;
        }

        public string ActCrazy()
        {
            return string.Format(Properties.Resources.Duck_ActCrazy, Name);
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
