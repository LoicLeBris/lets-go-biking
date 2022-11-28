using System;
using System.Collections.Generic;

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

            Console.WriteLine("Please enter an destination address :");
            string destination = Console.ReadLine();
            double[] destinationCoordinates = adresses.getAddressCoordinates(destination);

            int contractNumberOrigin = distance.findClosestContract(coordinates, originCoordinates);
            Console.WriteLine("Origin contract :");
            Console.WriteLine(contracts[contractNumberOrigin].ToString());

            int contractNumberDest = distance.findClosestContract(coordinates, destinationCoordinates);
            Console.WriteLine("Destination contract :");
            Console.WriteLine(contracts[contractNumberDest].ToString());

            List<Station> stationsOrigin = jcd.loadStations(contracts[contractNumberOrigin]);
            List<double[]> stationsCoordinates = jcd.getCoordinatesForEachStation(true, stationsOrigin);
            int stationIndex = distance.getShortestDistance(stationsCoordinates, originCoordinates, "foot-walking");
            Console.WriteLine("Origin Station:");
            Console.WriteLine(stationsOrigin[stationIndex]);

            List<Station> stationsDest = stationsOrigin;
            if (!contracts[contractNumberOrigin].Equals(contracts[contractNumberDest]))
            {
                stationsDest = jcd.loadStations(contracts[contractNumberDest]);
                stationsCoordinates = jcd.getCoordinatesForEachStation(true, stationsOrigin);
            }
            
            int stationIndexDest = distance.getShortestDistance(stationsCoordinates, destinationCoordinates, "foot-walking");
            Console.WriteLine("Destination Station:");
            Console.WriteLine(stationsDest[stationIndexDest]);

            Console.ReadLine();
        }
    }
}
