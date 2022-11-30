using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Device.Location;
using System.Collections;
using System.Web;
using System.Security.Policy;
using SOAPServer.ServiceReference1;

namespace SOAPServer
{
    internal class Program
    {
        static readonly Service1Client proxyCache = new Service1Client();

        public string method(string o, string d)
        {
            List<Contracts> contracts = JsonSerializer.Deserialize<List<Contracts>>(proxyCache.getContracts());
            List<Station> stations = JsonSerializer.Deserialize<List<Station>>(proxyCache.getStations());

            Distances distance = new Distances(stations);
            Adresses adresses = new Adresses();

            Console.WriteLine("Please enter an adress of origin :");
            GeoCoordinate origin = adresses.askForAdress(o);
            Console.WriteLine("Please enter an adress of destination :");
            GeoCoordinate destination = adresses.askForAdress(d);

            Station departureStation = distance.getShortestDistanceToStation(origin);
            Station arrivalStation = distance.getShortestDistanceToStation(destination);

            List<string> instructions = new List<string>();

            instructions.Add("La station la plus proche est :" + departureStation.ToString());

            Itineraire itineraire = new Itineraire(origin, destination, departureStation, arrivalStation);

            addInstructions(instructions, itineraire.getItineraryToDepartureStation());

            instructions.Add("Vous pouvez récupérer un vélo à la station. Il y a de la route jusqu'à la prochaine station : " + arrivalStation.ToString());

            addInstructions(instructions, itineraire.getItineraryToArrivalStation());

            instructions.Add("Laissez votre vélo ici et finissez le chemin à pied :");

            addInstructions(instructions, itineraire.getItineraryToDestinationAdress());

            return JsonSerializer.Serialize(instructions);
        }

        private void addInstructions(List<string> instructions, Steps[] steps)
        {
            foreach(Steps step in steps)
            {
                instructions.Add(step.instruction + " on " + step.distance + "m");
            }
        }
    }
}
