using System.Collections.Generic;

namespace PoliceMP.Garage.Shared

{
    /*"Model": "addpolbmwg30",
		"Name": "BMW G30",
		"Category": "RPU" */
    public class CarData
    {
        public string Model { get; set; }

        public string Name { get; set; }

        public List<string> Categories { get; set; }
    }
}
