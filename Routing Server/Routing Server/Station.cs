using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    public class Station
    {
        public Station()
        {
        }

        public int number { get; set; }
        public string name { get; set; }
        public string contractName { get; set; }
        public string address { get; set; }
        public Position position { get; set; }

        public Stands totalStands { get; set; }

        public override string ToString()
        {
            return "" + name + " at : " + address;
        }
    }

    public partial class Stands
    {
        public Availabilities availabilities { get; set; }

        public long capacity { get; set; }
    }

    public partial class Availabilities
    {
        public long bikes { get; set; }

        public long stands { get; set; }
    }

    public class Position
    {
        public double latitude { get; set; }
        public double longitude { get; set; }

        public override string ToString()
        {
            return longitude.ToString().Replace(',', '.') + "," + latitude.ToString().Replace(',', '.');
        }
    }
}
