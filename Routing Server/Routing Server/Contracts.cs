using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    public class Contracts
    {
        public Contracts()
        {
        }

        public string name { get; set; }

        public override string ToString()
        {
            return "" + name;
        }
    }
}
