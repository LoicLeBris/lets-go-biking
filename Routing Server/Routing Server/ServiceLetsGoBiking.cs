using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    public class ServiceLetsGoBiking : IServiceLetsGoBiking
    {
        public string GetItinerary(string a, string b)
        {
            Execution execution = new Execution();
            return execution.method(a, b);
        }
    }
}
