using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.Json;

namespace SOAPServer
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class ServiceLetsGoBiking : IServiceLetsGoBiking
    {
        public string GetItinerary(string a, string b)
        {
            Program program = new Program();
            return program.method(a, b);
        }
    }
}
