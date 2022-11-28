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
        
        public int getShortestDistance(List<double[]> coordinates)
        {
            string response = callMatrixEndpoint(coordinates, "foot-walking");
            string validString = response.Replace("null", "9.999");

            DurationMatrix durationMatrix = JsonConvert.DeserializeObject<DurationMatrix>(validString);

            List<List<double>> durations = new List<List<double>>();
            durations = durationMatrix.durations;
            
            List<double> dur = durations[0];
            dur.RemoveAt(0);

            int minIndex = -1;
            double minValue = 0.0;
            
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

            return minIndex+1;
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
