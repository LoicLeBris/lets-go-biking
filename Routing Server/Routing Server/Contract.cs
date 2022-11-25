using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    public class Contract
    {
        public Contract()
        {
        }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("cities", NullValueHandling = NullValueHandling.Ignore)]
        public string[] cities { get; set; }

        public override string ToString()
        {
            return "" + name;
        }
    }
}
