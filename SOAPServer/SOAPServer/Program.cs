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

namespace SOAPServer
{
    internal class Program
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        static readonly HttpClient client = new HttpClient();

        public string method(string o, string d)
        {
            string bikeUrl = "https://api.jcdecaux.com/vls/v3/";
            string query = "contracts?apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            string response = callApi(bikeUrl, query).Result;

            List<Steps> steps = new List<Steps>();
            List<string> instructions = new List<string>();
            List<Contracts> contracts = JsonSerializer.Deserialize<List<Contracts>>(response);
            List<Station> stations = new List<Station>();

            foreach (Contracts contract in contracts)
            {
                string queryStation = "stations?contract=" + contract + "&apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
                string responseStations = callApi(bikeUrl, queryStation).Result;
                List<Station> stationsOfContract = JsonSerializer.Deserialize<List<Station>>(responseStations);
                stations.AddRange(stationsOfContract);
            }

            Distances distance = new Distances(stations);
            Adresses adresses = new Adresses();

            Console.WriteLine("Please enter an adress of origin :");
            GeoCoordinate origin = adresses.askForAdress(o);
            Console.WriteLine("Please enter an adress of destination :");
            GeoCoordinate destination = adresses.askForAdress(d);

            Station departureStation = distance.getShortestDistanceToStation(origin);
            Station arrivalStation = distance.getShortestDistanceToStation(destination);

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

        static async Task<string> callApi(string url, string query)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage responseContractList = await client.GetAsync(url + query);
                responseContractList.EnsureSuccessStatusCode();
                string responseBody = await responseContractList.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);
                return responseBody;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return e.Message;
            }
        }
    }
}
