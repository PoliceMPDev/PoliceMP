using PoliceMP.Core.Client.Interface;
using PoliceMP.Core.Shared;

namespace PoliceMP.Client.Overlays.Legacy.IdCardOverlay
{
    public class IdCardOverlay : LegacyOverlay
    {
        public IdCardOverlay(ILegacyNuiManager nuiManager, ILogger<LegacyOverlay> logger) : base(nuiManager, logger)
        {
        }

        public void Show(IdCard idCard)
        {
            Show();

            string type = idCard.Type switch
            {
                IdCardType.Driver => "driver",
                IdCardType.Weapon => "weapon",
                _ => "none"
            };

            Emit("showIdCard", new
            {
                type = type,
                firstName = idCard.FirstName,
                lastName = idCard.LastName,
                isMale = idCard.IsMale,
                height = idCard.Height,
                dateOfBirth = idCard.DateOfBirth,
                drivingLicenseTypes = idCard.DrivingLicenseTypes.ToArray()
            });
        }

        public new void Hide()
        {
            base.Hide();
            Emit("closeIdCard");
        }
    }
}