using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    public class ServiceLetsGoBiking : IServiceLetsGoBiking
    {
        Execution execution = new Execution();
        public string GetItinerary(string a, string b)
        {
            execution.method(a, b);
            return "test";
        }
    }
}
