using System.Collections.Generic;

namespace PoliceMP.Shared.Options
{
    public class DiscordRichPresenceOptions
    {
        public string AppId { get; set; }
        public string Asset { get; set; }
        public string AssetText { get; set; }
        public string AssetSmall { get; set; }
        public string AssetSmallText { get; set; }
        public List<DiscordRichPresenceActionButton> ActionButtons { get; set; }
    }

    public class DiscordRichPresenceActionButton
    {
        public int Index { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
    }
}