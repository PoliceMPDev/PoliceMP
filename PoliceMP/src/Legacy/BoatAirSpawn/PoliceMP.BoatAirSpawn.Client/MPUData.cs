namespace PoliceMP.BoatAirSpawn.Client
{
    class MPUData
    {
        public MPUData(float _xMarker, float _yMarker, float _zMarker, float _xSpawn, float _ySpawn, float _zSpawn)
        {
            xmarker = _xMarker;
            ymarker = _yMarker;
            zmarker = _zMarker;

            xspawn = _xSpawn;
            yspawn = _ySpawn;
            zspawn = _zSpawn;
        }

        public float xmarker { get; set; }
        public float ymarker { get; set; }
        public float zmarker { get; set; }

        public float xspawn { get; set; }
        public float yspawn { get; set; }
        public float zspawn { get; set; }
    }
}
