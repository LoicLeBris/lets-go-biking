using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using System.Collections;
using System.Web;
using System.Security.Policy;
using System.Diagnostics.Contracts;

namespace Routing_Server
{
    internal class Program
    {

        static void Main(string[] args)
        {
            JCDecaux jcd = new JCDecaux();
            Adresses adresses = new Adresses();
            Distances distance = new Distances();

            List<Contract> contracts = jcd.loadContracts();
            
            List<double[]> coordinates = jcd.getCoordinatesForEachContract(adresses, contracts);

            Console.WriteLine("Please enter a departure address :");
            string origin = Console.ReadLine();
            double[] originCoordinates = adresses.getAddressCoordinates(origin);

            int contractNumber = distance.findClosestContract(coordinates, originCoordinates);
            
            Console.WriteLine("Closest contract :");
            Console.WriteLine(contracts[contractNumber].ToString());

            List<Station> stations = jcd.loadStations(contracts[contractNumber]);

            List<double[]> stationsCoordinates = jcd.getCoordinatesForEachStation(true, stations);

            int stationIndex = distance.getShortestDistance(stationsCoordinates, originCoordinates);

            Console.WriteLine(stations[stationIndex]);

            Console.ReadLine();
        }
    }
}
