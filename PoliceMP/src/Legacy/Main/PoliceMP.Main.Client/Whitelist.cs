using CitizenFX.Core;

namespace PoliceMP.Main.Client
{
    public class Whitelist : BaseScript
    {
        public static bool AFOStatus { get; private set; }
        public static bool RPUStatus { get; private set; }
        public static bool CIDStatus { get; private set; }
        public static bool NPASStatus { get; private set; }
        public static bool MPUStatus { get; private set; }
        public static bool AdminAuth { get; private set; }
        public static bool DogStatus { get; private set; }
        public static bool NHSStatus { get; private set; }
        public static bool FireStatus { get; private set; }
        public static bool ModAuth { get; private set; }
        public static bool WhitelistedMember { get; private set; }
        public static bool Developer { get; private set; }

        [EventHandler("PoliceMP:recieveSpecWhitelisting")]
        private void recieveSpecWhitelisting(bool afo, bool rpu, bool cid, bool npas, bool mpu, bool admin, bool dog, bool nhs, bool fire, bool mod,bool developer, bool whitelisted)
        {
            AFOStatus = afo;
            RPUStatus = rpu;
            CIDStatus = cid;
            NPASStatus = npas;
            MPUStatus = mpu;
            AdminAuth = admin;
            DogStatus = dog;
            NHSStatus = nhs;
            FireStatus = fire;
            ModAuth = mod;
            Developer = developer;
            WhitelistedMember = whitelisted;
        }
    }
}
