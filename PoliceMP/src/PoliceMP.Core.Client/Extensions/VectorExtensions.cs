using PoliceMP.Core.Shared.Models;

namespace PoliceMP.Core.Client.Extensions
{
    public static class VectorExtensions
    {
        public static PmpVector3 ToPmpVector3(this CitizenFX.Core.Vector3 vector3) => new PmpVector3(vector3.X, vector3.Y, vector3.Z);

        public static CitizenFX.Core.Vector3 ToCitizenVector3(this PmpVector3 pmpVector3) => new CitizenFX.Core.Vector3(pmpVector3.X, pmpVector3.Y, pmpVector3.Z);
    }
}
