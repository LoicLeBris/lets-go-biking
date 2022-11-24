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

        public GeoCoordinate askForAdress()
        {
            string originAdress = Console.ReadLine();

            string encodedAdress = HttpUtility.UrlEncode(originAdress);

            string queryAddress = "geocode/search?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&text=" + encodedAdress;
            string responseAddress = callApi(openRouteUrl, queryAddress).Result;

            OpenRouteService geocodeAddress = JsonSerializer.Deserialize<OpenRouteService>(responseAddress);
            double[] res = geocodeAddress.features[0].geometry.coordinates;

            double latitude = res[1];
            double longitude = res[0];

            GeoCoordinate adress = new GeoCoordinate(latitude, longitude);

            return adress;
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
