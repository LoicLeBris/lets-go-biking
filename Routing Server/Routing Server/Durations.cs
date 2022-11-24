using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Routing_Server
{
        public class DurationMatrix
        {
            [JsonProperty("durations")]
            public double[][] durations { get; set; }
        }
}
