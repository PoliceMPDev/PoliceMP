using PoliceMP.Main.Core.Shared.Models;

namespace PoliceMP.Main.Core.Client.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ToVector3(this CitizenFX.Core.Vector3 vector3) => new Vector3(vector3.X, vector3.Y, vector3.Z);

        public static CitizenFX.Core.Vector3 ToCitizenVector3(this Vector3 vector3) => new CitizenFX.Core.Vector3(vector3.X, vector3.Y, vector3.Z);
    }
}
