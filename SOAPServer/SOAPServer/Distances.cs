using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.UI.WebControls;

namespace SOAPServer
{
    public class Distances
    {
        private static readonly HttpClient client = new HttpClient();
        private List<Station> stations;
        
        public Distances(List<Station> stations) {
            this.stations = stations;
        }
        
        public Station getShortestDistanceToStation(GeoCoordinate origin)
        {
            List<double> distances = new List<double>();
            foreach (Station station in stations)
            {
                GeoCoordinate stationPos = new GeoCoordinate(station.position.latitude, station.position.longitude);
                double distance = stationPos.GetDistanceTo(origin);
                distances.Add(distance);
            }

            double shortest_distance = distances.Min();

            int i;
            for (i = 0; i < distances.Count; i++)
            {
                if (distances[i] == shortest_distance)
                {
                    break;
                }
            }

            return stations[i];
        }

    }
}
