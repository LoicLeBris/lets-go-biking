using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProxyCache
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class Service1 : IService1
    {
        private static HttpClient client = new HttpClient();
        static private GenericProxyCache proxyCache = new GenericProxyCache();
        public string getContracts()
        {
            JCDecauxItem item = proxyCache.Get<JCDecauxItem>("JCDecauxItem");
            return item.getContracts();
        }

        public string getStations()
        {
            JCDecauxItem item = proxyCache.Get<JCDecauxItem>("JCDecauxItem");
            return item.getStations();
        }

        public string getStationsByContractName(string contractName)
        {
            string queryStation = "stations?contract=" + contractName + "&apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
            return callApi(JCDecauxItem.apiUrl+queryStation).Result;
        }

        static public async Task<string> callApi(string url)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                HttpResponseMessage responseContractList = await client.GetAsync(url);
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

