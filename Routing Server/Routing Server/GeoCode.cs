using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    public class GeoCode
    {
        [JsonProperty("features", NullValueHandling = NullValueHandling.Ignore)]
        public Feature[] features { get; set; }
    }

    public class Feature
    {
        [JsonProperty("geometry", NullValueHandling = NullValueHandling.Ignore)]
        public Geometry geometry { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("coordinates", NullValueHandling = NullValueHandling.Ignore)]
        public double[] coordinates { get; set; }
    }
}
