using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Overlays.Legacy.AnprOverlay
{
    public class AnprOverlay : LegacyOverlay
    {
        public AnprOverlay(ILegacyNuiManager nuiManager, ILogger<LegacyOverlay> logger) : base(nuiManager, logger)
        {
        }

        public override void Show()
        {
            base.Show();
            Emit("showAlpr");
        }

        public override void Hide()
        {
            base.Hide();
            Emit("hideAlpr");
        }

        public void Lock(bool shouldLock)
        {
            if (shouldLock) Emit("lockAlpr");
            else Emit("unlockAlpr");
        }

        public void Update(AnprViewModel anprViewModel)
        {
            Emit("updateAlpr", anprViewModel);
        }

        public void Clear()
        {
            Emit("clearAlpr");
        }
    }
}