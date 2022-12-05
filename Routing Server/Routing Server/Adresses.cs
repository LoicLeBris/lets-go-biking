using Newtonsoft.Json.Linq;
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
        string token;
        public Adresses(string token) {
            this.token = token;
        }

        public double[] getAddressCoordinates(string address)
        {
            string encodedAdress = HttpUtility.UrlEncode(address);

            string queryAddress = "geocode/search?api_key="+ token +"&text=" + encodedAdress;
            string responseAddress = callApi(openRouteUrl, queryAddress).Result;

            GeoCode geocodeAddress = JsonSerializer.Deserialize<GeoCode>(responseAddress);
            if (geocodeAddress.features != null && geocodeAddress.features.Length != 0)
            {
                double[] res = geocodeAddress.features[0].geometry.coordinates;
                return res;
            }
            return null;
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
