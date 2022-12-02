using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Routing_Server
{
    [ServiceContract()]
    internal interface IServiceLetsGoBiking
    {
        [OperationContract()]
        string GetItinerary(string a, string b);

    }
}
