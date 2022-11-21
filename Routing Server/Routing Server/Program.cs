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

namespace Routing_Server
{
    internal class Program
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            string openRouteUrl = "https://api.openrouteservice.org/";
            string bikeUrl = "https://api.jcdecaux.com/vls/v3/";
            string query = "contracts?apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            string response = callApi(bikeUrl, query).Result;

            List<Contracts> contracts = JsonSerializer.Deserialize<List<Contracts>>(response);
            List<Station> stations = new List<Station>();

            foreach (Contracts contract in contracts)
            {
                string queryStation = "stations?contract=" + contract + "&apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
                string responseStations = callApi(bikeUrl, queryStation).Result;
                List<Station> stationsOfContract = JsonSerializer.Deserialize<List<Station>>(responseStations);
                stations.AddRange(stationsOfContract);
            }

            Console.WriteLine("Please enter an adress of origin :");
            string originAdress = Console.ReadLine();

            string encodedAdress = HttpUtility.UrlEncode(originAdress);

            string queryOriginAddress = "geocode/search?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&text=" + encodedAdress;
            string responseOriginAddress = callApi(openRouteUrl, queryOriginAddress).Result;

            OpenRouteService geocodeOriginAddress = JsonSerializer.Deserialize<OpenRouteService>(responseOriginAddress);
            double[] res = geocodeOriginAddress.features[0].geometry.coordinates;

            Console.WriteLine(res[0]);
            Console.WriteLine(res[1]);

            //GeoCoordinate myPos = new GeoCoordinate(myLatitude, myLongitude);


            /*List<double> distances = new List<double>();
            foreach (Station station in stations)
            {
                GeoCoordinate stationPos = new GeoCoordinate(station.position.latitude, station.position.longitude);
                //double distance = stationPos.GetDistanceTo(myPos);
                //distances.Add(distance);
            }*/

            /*double shortest_distance = distances.Min();
            Console.WriteLine("Shortest distance " + shortest_distance);*/

            /*int i;
            for (i = 0; i < distances.Count; i++)
            {
                if (distances[i] == shortest_distance)
                {
                    break;
                }
            }*/

            //Console.WriteLine("La station la plus proche est :" + stations[i].name);

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
