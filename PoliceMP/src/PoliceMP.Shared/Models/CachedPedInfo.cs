using System;

namespace PoliceMP.Shared.Models
{
    public class CachedPedInfo
    {
        public PedInfo PedInfo { get; }
        public DateTime CachedAt { get; }

        public CachedPedInfo(PedInfo pedInfo)
        {
            PedInfo = pedInfo;
            CachedAt = DateTime.Now;
        }
    }
}