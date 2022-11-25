using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.Json.Nodes;
using System.Runtime.Remoting.Messaging;
using static System.Collections.Specialized.BitVector32;

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

            DurationMatrix durationMatrix = System.Text.Json.JsonSerializer.Deserialize<DurationMatrix>(response);
            double[][] durations = durationMatrix.durations;

            double[] dur = durations[0];

            Console.WriteLine("Durations");
            for(int i = 0; i < dur.Length; i++)
            {
                Console.WriteLine(dur[i]);
            }
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

        public void getClosestCity(List<double[]> coordinates)
        {
            List<GeoCoordinate> geoCoordinates= new List<GeoCoordinate>();
            foreach(double[] coordinatesItem in coordinates)
            {
                GeoCoordinate pos = new GeoCoordinate(coordinatesItem[1], coordinatesItem[0]);
                geoCoordinates.Add(pos);
            }
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
