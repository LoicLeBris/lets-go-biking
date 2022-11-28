using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.SqlServer.Server;

namespace Routing_Server
{
    public class JCDecaux
    {
        static readonly HttpClient client = new HttpClient();
        string apiUrl = "https://api.jcdecaux.com/vls/v3/";

        public JCDecaux()
        {
            
        }

        public List<Contract> loadContracts()
        {
            string contractsEndpoint = "contracts?apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            string response = callApi(apiUrl, contractsEndpoint).Result;
            List<Contract> contracts = JsonSerializer.Deserialize<List<Contract>>(response);
            return contracts;
        }

        public List<double[]> getCoordinatesForEachContract(Adresses adresses, List<Contract> contracts)
        {
            List<double[]> coordinates = new List<double[]>();

            foreach (Contract contract in contracts)
            {
                if (contract.name != null)
                {
                    double[] cityCoordinates = adresses.getAddressCoordinates(contract.name);
                    if (cityCoordinates != null)
                    {
                        coordinates.Add(cityCoordinates);
                    }
                }
            }

            return coordinates;
        }

        public List<Station> loadStations(Contract contract)
        {
            List<Station> stations = new List<Station>();
            string queryStation = "stations?contract=" + contract.name + "&apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            string responseStations = callApi(apiUrl, queryStation).Result;
            List<Station> stationsOfContract = JsonSerializer.Deserialize<List<Station>>(responseStations);
            stations.AddRange(stationsOfContract);
            return stations;
        }

        public List<double[]> getCoordinatesForEachStation(bool isPickingUpABike, List<Station> stations)
        {
            List<double[]> stationsCoordinates = new List<double[]>();

            if (isPickingUpABike)
            {
                foreach (Station station in stations)
                {
                    if (station.totalStands.availabilities.bikes > 0)
                    {
                        double[] stationCoordinates = { station.position.longitude, station.position.latitude };
                        stationsCoordinates.Add(stationCoordinates);
                    }
                }
            }
            else
            {
                foreach (Station station in stations)
                {
                    if (station.totalStands.availabilities.stands > 0)
                    {
                        double[] stationCoordinates = { station.position.longitude, station.position.latitude };
                        stationsCoordinates.Add(stationCoordinates);
                    }
                }
            }
           
            return stationsCoordinates;
        }
            static async Task<string> callApi(string url, string query)
        {
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
