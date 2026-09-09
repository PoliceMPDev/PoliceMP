using System.IO;

namespace PoliceMP.Garage.Server
{
    public static class CarReader
    {
        private const string FILE_LOCATION = "Cars.json";

        private static string _carsString;

        public static string All()
        {
            if (_carsString != null && _carsString.Length != 0) return _carsString;

            using (var file = File.OpenText(FILE_LOCATION))
            {
                _carsString = file.ReadToEnd();
            }

            return _carsString;
        }
    }
}