using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    public class OpenRouteService
    {
        public Feature[] features { get; set; }
    }

    public class Feature
    {
        public Geometry geometry { get; set; }  
    }

    public class Geometry
    {
        public double[] coordinates { get; set; }
    }
}
