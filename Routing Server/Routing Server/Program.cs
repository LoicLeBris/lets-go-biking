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

namespace Routing_Server
{
    internal class Program
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {

            string url = "https://api.jcdecaux.com/vls/v3/";
            string query = "contracts?apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            string response = callApi(url, query).Result;

            List<Contracts> contracts = JsonSerializer.Deserialize<List<Contracts>>(response);

            foreach (Contracts contract in contracts)
            {
                Console.WriteLine(contract.ToString());
            }

            Console.WriteLine("Please choose a contract :");
            string chosenContract = Console.ReadLine();

            string queryStation = "stations?contract=" + chosenContract + "&apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            string responseStations = callApi(url, queryStation).Result;

            string adress = "186 chemin des génissieux Saint-Ismier";

            string encodedAdress = HttpUtility.UrlEncode(adress, );


            List<Station> stations = JsonSerializer.Deserialize<List<Station>>(responseStations);

            double myLatitude = 43.619214;
            double myLongitude = 7.0760172;
            GeoCoordinate myPos = new GeoCoordinate(myLatitude, myLongitude);


            List<double> distances = new List<double>();
            foreach (Station station in stations)
            {
                GeoCoordinate stationPos = new GeoCoordinate(station.position.latitude, station.position.longitude);
                double distance = stationPos.GetDistanceTo(myPos);
                distances.Add(distance);
            }

            double shortest_distance = distances.Min();
            Console.WriteLine("Shortest distance " + shortest_distance);

            int i;
            for (i = 0; i < distances.Count; i++)
            {
                if (distances[i] == shortest_distance)
                {
                    break;
                }
            }

            Console.WriteLine("La station la plus proche est :" + stations[i].name);

            Console.ReadLine();

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
