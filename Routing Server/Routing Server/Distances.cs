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

namespace Routing_Server
{
    public class Distances
    {
        private static readonly HttpClient client = new HttpClient();
        private List<Station> stations;
        
        public Distances(List<Station> stations) {
            this.stations = stations;
        }
        
        public void getShortestDistanceToStation(double[] origin)
        {
            string response = callMatrixEndpoint(origin, "foot-walking");

            DurationMatrix durationMatrix = System.Text.Json.JsonSerializer.Deserialize<DurationMatrix>(response);
            double[][] durations = durationMatrix.durations;

            double[] dur = durations[0];

            Console.WriteLine("Durations");
            for(int i = 0; i < dur.Length; i++)
            {
                Console.WriteLine(dur[i]);
            }
        }

        public string callMatrixEndpoint(double[] origin, string type)
        {
            string url = "https://api.openrouteservice.org/v2/matrix/";
            client.DefaultRequestHeaders.Add("Authorization", "5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2");

            List<double[]> coordinates = new List<double[]>();
            coordinates.Add(origin);

            int i = 0;
            foreach (Station station in stations)
            {
                i++;
                double[] distance = new double[2];
                distance[0] = station.position.longitude;
                distance[1] = station.position.latitude;
                coordinates.Add(distance);
                if (i == 20) { break; }
            }

            var locations = new { locations = coordinates };
            var output = JsonConvert.SerializeObject(locations);

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
