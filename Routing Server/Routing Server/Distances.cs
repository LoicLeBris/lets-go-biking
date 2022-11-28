using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Xml;
using System.Collections;
using System.Web.UI;

namespace Routing_Server
{
    public class Distances
    {
        private static readonly HttpClient client = new HttpClient();
        
        public Distances() {
            client.DefaultRequestHeaders.Add("Authorization", "5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2");
        }

        public int getShortestDistance(List<double[]> coordinates, double[] userInput)
        {
            List<List<double>> durations = new List<List<double>>();
            List<double> dur = new List<double>();
            List<List<double[]>> chunkedStations = ChunkBy(coordinates, 30);

            foreach (var chunked in chunkedStations)
            {
                chunked.Insert(0, userInput);
                string response = callMatrixEndpoint(chunked, "foot-walking");
                string validString = response.Replace("null", "9.999");
                DurationMatrix durationMatrix = JsonConvert.DeserializeObject<DurationMatrix>(validString);
                durations = durationMatrix.durations;
                durations[0].RemoveAt(0);
                dur.AddRange(durations[0]);
            }

            foreach(var item in dur)
            {
                Console.WriteLine(item);
            }
            
            int minIndex = -1;
            double minValue = dur.Max();
            
            for(int i = 0; i < dur.Count; i++)
            {
                if (dur[i] != 9.999)
                {
                    if (dur[i] <= minValue)
                    {
                        minValue = dur[i];
                        minIndex= i;
                    }
                }
            }

            if(minIndex == -1)
            {
                Console.WriteLine("Désolé, nous n'avons pas trouvé de stations suffisament proche de vous pour y aller à pied");
                Console.ReadLine();
            }

            return minIndex;
        }

        public int findClosestContract(List<double[]> coordinates, double[] userInput)
        {
            GeoCoordinate userCoordinate = new GeoCoordinate(userInput[1], userInput[0]);

            List<double> distances = new List<double>();
            foreach (var coordinate in coordinates)
            {
                GeoCoordinate contractPos = new GeoCoordinate(coordinate[1], coordinate[0]);
                double distance = contractPos.GetDistanceTo(userCoordinate);
                distances.Add(distance);
            }

            double shortest_distance = distances.Min();

            int i = 0;
            for (; i < distances.Count; i++)
            {
                if (distances[i] == shortest_distance)
                {
                    break;
                }
            }
            
            return i+1;
        }


        public string callMatrixEndpoint(List<double[]> coordinates, string type)
        {
            string url = "https://api.openrouteservice.org/v2/matrix/";

            var locations = new { locations = coordinates };
            var output = JsonConvert.SerializeObject(locations);

            Console.WriteLine(output);

            var locationsString = new StringContent(output, Encoding.UTF8, "application/json");

            string response = callApi(url, type, locationsString).Result;
          
            return response;
        }

        public List<List<T>> ChunkBy<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }


        static async Task<string> callApi(string url, string query, StringContent content)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage responseContractList = await client.PostAsync(url + query, content);
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
