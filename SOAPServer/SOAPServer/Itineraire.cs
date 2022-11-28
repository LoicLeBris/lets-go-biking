using System;
using System.Device.Location;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SOAPServer
{
    internal class Itineraire
    {
        static readonly HttpClient client = new HttpClient();
        string openRouteUrl = "https://api.openrouteservice.org/";

        private GeoCoordinate origin;
        private GeoCoordinate destination;
        private Station departureStation;
        private Station arrivalStation;

        public Itineraire(GeoCoordinate origin, GeoCoordinate destination, Station departureStation, Station arrivalStation)
        {
            this.origin = origin;
            this.destination = destination;
            this.departureStation = departureStation;
            this.arrivalStation = arrivalStation;
        }

        public Steps[] getItineraryToDepartureStation()
        {
            string originPosition = origin.Longitude.ToString().Replace(',', '.') + "," + origin.Latitude.ToString().Replace(',', '.');
            string queryItinerary = "v2/directions/foot-walking?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&start="+originPosition+ "&end=" + departureStation.position.ToString();

            string responseAddress = callApi(openRouteUrl, queryItinerary).Result;

            Directions directions = JsonSerializer.Deserialize<Directions>(responseAddress);
            Steps[] steps = directions.features[0].properties.segments[0].steps;

            return steps;
        }

        public Steps[] getItineraryToArrivalStation()
        {
            string queryItinerary = "v2/directions/cycling-regular?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&start=" + departureStation.position.ToString() + "&end=" + arrivalStation.position.ToString();

            string responseAddress = callApi(openRouteUrl, queryItinerary).Result;

            Directions directions = JsonSerializer.Deserialize<Directions>(responseAddress);
            Steps[] steps = directions.features[0].properties.segments[0].steps;

            return steps;
        }

        public Steps[] getItineraryToDestinationAdress()
        {
            string destinationPosition = destination.Longitude.ToString().Replace(',', '.') + "," + destination.Latitude.ToString().Replace(',', '.');
            string queryItinerary = "v2/directions/cycling-regular?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&start=" + arrivalStation.position.ToString() + "&end=" + destinationPosition;

            string responseAddress = callApi(openRouteUrl, queryItinerary).Result;

            Directions directions = JsonSerializer.Deserialize<Directions>(responseAddress);
            Steps[] steps = directions.features[0].properties.segments[0].steps;

            return steps;
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