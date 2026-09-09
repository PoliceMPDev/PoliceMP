namespace PoliceMP.Shared.Constants.States
{
    public static class PlayerStates
    {
        public const string HasSpawned = "PlayerState:HasSpawned";
        public const string LastNameFromAskForId = "PlayerState:LastNameFromAskForId";
        public const string LastPlateFromPullover = "PlayerState:LastPlateFromPullover";
        public const string HideBlipState = "pmphideblips";
        public const string CurrentRole = "currentRole";
        public const string CallSign = "policemp:callsign";

        public static class PlayerTags
        {
            public const string Enabled = "policemp.playertags.enabled";
            public const string SelfEnabled = "policemp.playertags.selfEnabled";
            public const string ModOverride = "policemp.playertags.modOverride";
            public const string Scale = "policemp.playertags.scale";
        }
    }
}