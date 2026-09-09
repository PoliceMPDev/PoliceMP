using CitizenFX.Core;

namespace PoliceMP.Main.Client.Util
{
    public class SoundPlayer : BaseScript
    {
        public const string DoubleBeep = "double_beep";
        public const string SingleBeep = "single_beep1";
        public const string Cuff = "cuff";
        public const string Snake = "snake";
        public const string Inhaler = "inhaler";
        public const string PatDown = "patdown";
        public const string SearchCar = "search_car";
        public const string LevelUp = "levelup";
        public const string Trombone = "trombone";
        public const string Yay = "yay";
        public const string Attention = "attention";

        public static void PlaySound(string file, int volume = 10)
        {
            if (volume > 10) volume = 10;
            else if (volume < 0) volume = 0;

            TriggerEvent("playSound", file, volume);
        }
    }
}
