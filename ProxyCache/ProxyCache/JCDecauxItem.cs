using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProxyCache
{
    internal class JCDecauxItem
    {
        public static string apiUrl = "https://api.jcdecaux.com/vls/v3/";
        public static string apiKey = "apiKey=a20510ebde21e2f45630b65733ea766ea9a88778";
        private string contractsJson;
        private string stationsJson;
        public JCDecauxItem()
        {
            contractsJson = Service1.callApi(apiUrl + "contracts?" + apiKey).Result;
            stationsJson = Service1.callApi(apiUrl + "stations?" + apiKey).Result;
        }

        public string getContracts()
        {
            return contractsJson;
        }

        public string getStations()
        {
            return stationsJson;
        }

        public override string ToString()
        {
            return "JCDecauxItem";
        }
    }
}
