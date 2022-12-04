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
            if (client.DefaultRequestHeaders.Count() == 0)
            {
                client.DefaultRequestHeaders.Add("Authorization", "5b3ce3597851110001cf6248068382780acc46bc940a29b5ce9e693f");
            }                       
        }

        public int getShortestDistance(List<double[]> coordinates, double[] userInput, string travelMethod)
        {
            List<List<double>> durations = new List<List<double>>();            
            List<List<double[]>> chunkedStations = ChunkBy(coordinates, 30);
            List<double> dur = new List<double>();
            List<Task<string>> tasks = new List<Task<string>>();
            foreach(var chunked in chunkedStations)
            {
                chunked.Insert(0, userInput);
                Task<string> result = callMatrixEndpoint(chunked, travelMethod);
                tasks.Add(result);
            }

            foreach (var task in tasks)
            {
                task.Wait();
                string response = task.Result;
                string validString = response.Replace("null", "9.999");
                DurationMatrix durationMatrix = JsonConvert.DeserializeObject<DurationMatrix>(validString);
                durations = durationMatrix.durations;
                durations[0].RemoveAt(0);
                dur.AddRange(durations[0]);
            }

            int minIndex = -1;
            double minValue = dur.Min();
            if(minValue != null)
            {
                minIndex = dur.IndexOf(minValue);
            }         

            if(minIndex == -1)
            {
                Console.WriteLine("Désolé, nous n'avons pas trouvé de stations suffisament proche de vous pour y aller à pied");
                Console.ReadLine();
            }
           

            return minIndex;
        }

        public List<double> getListOfDurationsPerStation(List<double[]> coordinates, double[] userInput, string travelMethod)
        {
            List<List<double>> durations = new List<List<double>>();
            List<List<double[]>> chunkedStations = ChunkBy(coordinates, 30);
            List<double> dur = new List<double>();
            List<Task<string>> tasks = new List<Task<string>>();
            foreach (var chunked in chunkedStations)
            {
                chunked.Insert(0, userInput);
                Task<string> result = callMatrixEndpoint(chunked, travelMethod);
                tasks.Add(result);
            }

            foreach (var task in tasks)
            {
                task.Wait();
                string response = task.Result;
                string validString = response.Replace("null", "9.999");
                DurationMatrix durationMatrix = JsonConvert.DeserializeObject<DurationMatrix>(validString);
                durations = durationMatrix.durations;
                durations[0].RemoveAt(0);
                dur.AddRange(durations[0]);
            }

            return dur;
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


        public Task<string> callMatrixEndpoint(List<double[]> coordinates, string type)
        {
            string url = "https://api.openrouteservice.org/v2/matrix/";

            var locations = new { locations = coordinates };
            var output = JsonConvert.SerializeObject(locations);

            var locationsString = new StringContent(output, Encoding.UTF8, "application/json");

            Task<string> response = callApi(url, type, locationsString);
          
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
