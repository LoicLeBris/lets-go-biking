using Routing_Server.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text.Json;

namespace Routing_Server
{
    internal class Execution
    {
        static readonly Service1Client proxyCache = new Service1Client();
        public string method(string origin, string destination)
        {
            Adresses adresses = new Adresses();
            Distances distance = new Distances();
            List<string> instructions = new List<string>();

            List<Contract> contracts = JsonSerializer.Deserialize<List<Contract>>(proxyCache.getContracts());
            List<double[]> coordinates = getCoordinatesForEachContract(adresses, contracts);
           
            double[] originCoordinates = adresses.getAddressCoordinates(origin);                     
            double[] destinationCoordinates = adresses.getAddressCoordinates(destination);

            int contractNumberOrigin = distance.findClosestContract(coordinates, originCoordinates);
            int contractNumberDest = distance.findClosestContract(coordinates, destinationCoordinates);

            instructions.Add("Origin contract :" + contracts[contractNumberOrigin].ToString());                       
            instructions.Add("Destination contract :" + contracts[contractNumberDest].ToString());            

            List<Station> stationsOrigin = JsonSerializer.Deserialize<List<Station>>(proxyCache.getStationsByContractName(contracts[contractNumberOrigin].name));
            List<double[]> stationsCoordinates = getCoordinatesForEachStation(true, stationsOrigin);
            int stationIndex = distance.getShortestDistance(stationsCoordinates, originCoordinates, "foot-walking");
            instructions.Add("Origin Station:" + stationsOrigin[stationIndex]);

            List<Station> stationsDest = stationsOrigin;
            if (!contracts[contractNumberOrigin].Equals(contracts[contractNumberDest]))
            {
                stationsDest = JsonSerializer.Deserialize<List<Station>>(contracts[contractNumberDest].name);
                stationsCoordinates = getCoordinatesForEachStation(false, stationsOrigin);
            }
            
            int stationIndexDest = distance.getShortestDistance(stationsCoordinates, destinationCoordinates, "foot-walking");
            instructions.Add("Destination Station:" + stationsDest[stationIndexDest]);

            instructions.Add("La station la plus proche est :" + stationsOrigin[stationIndex].ToString());

            string originString = originCoordinates[0].ToString().Replace(',', '.') + "," + originCoordinates[1].ToString().Replace(',', '.');
            string destString = destinationCoordinates[0].ToString().Replace(',', '.') + "," + destinationCoordinates[1].ToString().Replace(',', '.');

            Itineraire itineraire = new Itineraire(originString, destString, stationsOrigin[stationIndex], stationsDest[stationIndexDest]);

            addInstructions(instructions, itineraire.getItineraryToDepartureStation());

            instructions.Add("Vous pouvez récupérer un vélo à la station. Il y a de la route jusqu'à la prochaine station : " + stationsDest[stationIndexDest].ToString());

            addInstructions(instructions, itineraire.getItineraryToArrivalStation());

            instructions.Add("Laissez votre vélo ici et finissez le chemin à pied :");

            addInstructions(instructions, itineraire.getItineraryToDestinationAdress());

            return JsonSerializer.Serialize(instructions);


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

        private void addInstructions(List<string> instructions, Steps[] steps)
        {
            foreach (Steps step in steps)
            {
                instructions.Add(step.instruction + " on " + step.distance + "m");
            }
        }
    }    
}
