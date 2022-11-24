using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Routing_Server
{
    public class Adresses
    {
        static readonly HttpClient client = new HttpClient();
        string openRouteUrl = "https://api.openrouteservice.org/";

        public Adresses() { }

        public double[] askForOrigin()
        {
            Console.WriteLine("Please enter an adress of origin :");
            string originAdress = Console.ReadLine();

            string encodedAdress = HttpUtility.UrlEncode(originAdress);

            string queryOriginAddress = "geocode/search?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&text=" + encodedAdress;
            string responseOriginAddress = callApi(openRouteUrl, queryOriginAddress).Result;

            OpenRouteService geocodeOriginAddress = JsonSerializer.Deserialize<OpenRouteService>(responseOriginAddress);
            double[] res = geocodeOriginAddress.features[0].geometry.coordinates;

            /*double latitude = res[1];
            double longitude = res[0];

            double[] coordinates = new double[2];
            coordinates[0] = latitude;
            coordinates[1] = longitude;*/

            return res;
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
