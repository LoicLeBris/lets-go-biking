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

        public string name { get; set; }
        public string contractName { get; set; }
        public string address { get; set; }
        public Position position { get; set; }

        public override string ToString()
        {
            return "name : " + name 
                + "\naddress :" + address 
                + "\ncontractName :" + contractName;
        }
    }

    public class Position
    {
        public double latitude { get; set; }
        public double longitude { get; set; }

        public override string ToString()
        {
            return longitude.ToString().Replace(',', '.')+ "," + latitude.ToString().Replace(',', '.');
        }
    }
}
