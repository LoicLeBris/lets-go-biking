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

namespace Routing_Server
{
    public class Distances
    {
        private static readonly HttpClient client = new HttpClient();
        
        public Distances() {
          
        }
        
        public void getShortestDistance(List<double[]> coordinates)
        {
            string response = callMatrixEndpoint(coordinates, "foot-walking");

            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            DurationMatrix durationMatrix = JsonConvert.DeserializeObject<DurationMatrix>(response, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            List<List<double?>> durations = new List<List<double?>>();
            durations = durationMatrix.durations;
            
            List<double?> dur = durations[0];
            dur.RemoveAt(0);

            Console.WriteLine("Durations :");

            double? minima = 0;
            int mindex = 0;

            for (int i = 0; i < dur.Count; i++)
            {
                if (dur[i] < minima)
                { 
                    minima = dur[i]; 
                    mindex = i; 
                }
            }

            Console.WriteLine(mindex);
        }

        public string callMatrixEndpoint(List<double[]> coordinates, string type)
        {
            string url = "https://api.openrouteservice.org/v2/matrix/";
            client.DefaultRequestHeaders.Add("Authorization", "5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2");

            var locations = new { locations = coordinates };
            var output = JsonConvert.SerializeObject(locations);

            Console.WriteLine(output);

            var locationsString = new StringContent(output, Encoding.UTF8, "application/json");

            string response = callApi(url, type, locationsString).Result;
          
            return response;
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
