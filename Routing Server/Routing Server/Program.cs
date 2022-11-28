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
            List<Contract> contracts = jcd.loadContracts();
            
            Adresses adresses = new Adresses();

            List<double[]> coordinates = new List<double[]>();

            Console.WriteLine("Please enter a departure address :");
            string origin = Console.ReadLine();
            double[] originCoordinates = adresses.getAddressCoordinates(origin);
            
            foreach (Contract contract in contracts)
            {
                if(contract.name != null)
                {
                    double[] cityCoordinates = adresses.getAddressCoordinates(contract.name);
                    if (cityCoordinates != null)
                    {
                        coordinates.Add(cityCoordinates);
                    }
                }
            }

            Distances distance = new Distances();
            int contractNumber = distance.findClosestContract(coordinates, originCoordinates);
            
            Console.WriteLine("Contrat le plus proche :");
            Console.WriteLine(contracts[contractNumber].ToString());

            List<Station> stations = jcd.loadStations(contracts[contractNumber]);

            List<double[]> stationsCoordinates = new List<double[]>();
            foreach(Station station in stations) {
                double[] stationCoordinates = {station.position.longitude, station.position.latitude};
                stationsCoordinates.Add(stationCoordinates);
            }


            int stationIndex = distance.getShortestDistance(stationsCoordinates, originCoordinates);


            Console.WriteLine(stations[stationIndex]);

            Console.ReadLine();
        }

        


    }
}
