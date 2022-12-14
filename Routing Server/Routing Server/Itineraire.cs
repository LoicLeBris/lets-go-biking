using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Routing_Server
{
    internal class Itineraire
    {
        static readonly HttpClient client = new HttpClient();
        string openRouteUrl = "https://api.openrouteservice.org/";

        private string origin;
        private string destination;
        private Station departureStation;
        private Station arrivalStation;

        public Itineraire(string origin, string destination, Station departureStation, Station arrivalStation)
        {
            this.origin = origin;
            this.destination = destination;
            this.departureStation = departureStation;
            this.arrivalStation = arrivalStation;
        }

        public Directions getItineraryToDepartureStation()
        { 
            string queryItinerary = "v2/directions/foot-walking?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&start=" + origin + "&end=" + departureStation.position.ToString();            
            string responseAddress = callApi(openRouteUrl, queryItinerary).Result;

            Directions directions = JsonSerializer.Deserialize<Directions>(responseAddress);
            return directions;
        }

        public Directions getItineraryToArrivalStation()
        {
            string queryItinerary = "v2/directions/cycling-regular?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&start=" + departureStation.position.ToString() + "&end=" + arrivalStation.position.ToString();

            string responseAddress = callApi(openRouteUrl, queryItinerary).Result;

            Directions directions = JsonSerializer.Deserialize<Directions>(responseAddress);
            return directions;
        }

        public Directions getItineraryToDestinationAdress()
        {           
            string queryItinerary = "v2/directions/cycling-regular?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&start=" + arrivalStation.position.ToString() + "&end=" + destination;

            string responseAddress = callApi(openRouteUrl, queryItinerary).Result;

            Directions directions = JsonSerializer.Deserialize<Directions>(responseAddress);           

            return directions;
        }

        public List<Directions> GetDirections()
        {
            List<Directions> directions = new List<Directions>();
            directions.Add(getItineraryToDepartureStation());
            directions.Add(getItineraryToArrivalStation());
            directions.Add(getItineraryToDestinationAdress());
            return directions;
        }       
        

        public List<Directions> getItineraryFromOriginToDestinationByWalk()
        {
            string queryItinerary = "v2/directions/foot-walking?api_key=5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2&start=" + origin + "&end=" + destination;

            string responseAddress = callApi(openRouteUrl, queryItinerary).Result;
            List<Directions> list = new List<Directions>();
            Directions directions = JsonSerializer.Deserialize<Directions>(responseAddress);
            list.Add(directions);
            return list;
        }

        public double getDuration(List<Directions> directions)
        {
            double duration = 0;
            foreach (Directions direction in directions)
            {
                Console.WriteLine("Temps de chaque segment :" + direction.features[0].properties.segments[0].duration);
                duration += direction.features[0].properties.segments[0].duration;
            }

            return duration;
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
