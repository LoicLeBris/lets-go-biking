using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    internal class Directions
    {
        public Feature[] features { get; set; }

        public class Feature
        {
            public Properties properties { get; set; }
        }

        public class Properties
        {
            public Segments[] segments { get; set; }
        }

        public class Segments
        {
            public double distance { get; set; }
            public double duration { get; set; }
            public Steps[] steps { get; set; }
        }
    }
    public class Steps
    {
        public double distance { get; set; }
        public double duration { get; set; }
        public string instruction { get; set; }
    }
}
