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
        List<Contract> contracts;
        List<Station> stations;

        public JCDecaux()
        {
            contracts = new List<Contract>();
            stations = new List<Station>();
        }

        public List<Contract> loadContracts()
        {
            string contractsEndpoint = "contracts?apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            string response = callApi(apiUrl, contractsEndpoint).Result;
            List<Contract> contracts = JsonSerializer.Deserialize<List<Contract>>(response);
            return contracts;
        }

        public List<Station> loadStations(Contract contract)
        {
            stations = new List<Station>();
            string queryStation = "stations?contract=" + contract.name + "&apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            string responseStations = callApi(apiUrl, queryStation).Result;
            List<Station> stationsOfContract = JsonSerializer.Deserialize<List<Station>>(responseStations);
            stations.AddRange(stationsOfContract);
            return stations;
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
