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
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.

        static void Main(string[] args)
        {
            JCDecaux jcd = new JCDecaux();
            List<Contract> contracts = jcd.loadContracts();
            
            Adresses adresses = new Adresses();

            List<double[]> coordinates = new List<double[]>();

            Console.WriteLine("Please enter a departure address :");
            string origin = Console.ReadLine();
            double[] originCoordinates = adresses.getAddressCoordinates(origin);
            coordinates.Add(originCoordinates);

            foreach (Contract contract in contracts)
            {
                if(contract.name != null)
                {
                    double[] cityCoordinates = adresses.getAddressCoordinates(contract.name);
                    if (cityCoordinates != null)
                    {
                        Console.WriteLine(contract.name);
                        coordinates.Add(cityCoordinates);
                    }
                }
            }



            Distances distance = new Distances();
            distance.getShortestDistance(coordinates);

            /*
            double[] origin = adresses.askForOrigin();
            
            distance.getShortestDistanceToStation(origin);*/

            //Console.WriteLine("La station la plus proche est :" + departureStation.ToString());

            Console.ReadLine();
        }

        
    }
}
