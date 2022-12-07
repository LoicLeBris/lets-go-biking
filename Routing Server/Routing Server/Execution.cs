using Routing_Server.ServiceReference1;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.Json;

namespace Routing_Server
{
    internal class Execution
    {
        static readonly Service1Client proxyCache = new Service1Client();
        string token1 = "5b3ce3597851110001cf62482172e1aa1d5a469c9e68b05c8e06cfe2";
        string token2 = "5b3ce3597851110001cf62486b7528a4b8dc45c38139ff0899fa358f";
        public void method(string origin, string destination)
        {
            Adresses adresses = new Adresses(token1);
            Distances distance = new Distances(token2);           
            ActiveMq activemq = new ActiveMq();

            activemq.sendMessage("We are currently considering your request... Please wait...");

            List<Contract> contracts = JsonSerializer.Deserialize<List<Contract>>(proxyCache.getContracts());
            List<double[]> coordinates = getCoordinatesForEachContract(adresses, contracts);
           
            double[] originCoordinates = adresses.getAddressCoordinates(origin);                     
            double[] destinationCoordinates = adresses.getAddressCoordinates(destination);

            int contractNumberOrigin = distance.findClosestContract(coordinates, originCoordinates);
            int contractNumberDest = distance.findClosestContract(coordinates, destinationCoordinates);            

            activemq.sendMessage("Origin contract :" + contracts[contractNumberOrigin].ToString());            
            activemq.sendMessage("Destination contract :" + contracts[contractNumberDest].ToString());

            List<Station> stationsOrigin = JsonSerializer.Deserialize<List<Station>>(proxyCache.getStationsByContractName(contracts[contractNumberOrigin].name));
            List<double[]> stationsCoordinates = getCoordinatesForEachStation(true, stationsOrigin);           

            List<double> durationsByStation = distance.getListOfDurationsPerStation(stationsCoordinates, originCoordinates, "foot-walking");
            int indexOrigin = durationsByStation.IndexOf(durationsByStation.Min());

            while (proxyCache.isABikeAvailableInStation(JsonSerializer.Serialize(stationsOrigin[indexOrigin])) == false)
            {
                durationsByStation[indexOrigin] = durationsByStation.Max();                
                indexOrigin = durationsByStation.IndexOf(durationsByStation.Min());
            }                        

            List<Station> stationsDest = stationsOrigin;
            if (!contracts[contractNumberOrigin].Equals(contracts[contractNumberDest]))
            {
                stationsDest = JsonSerializer.Deserialize<List<Station>>(proxyCache.getStationsByContractName(contracts[contractNumberDest].name));
                stationsCoordinates = getCoordinatesForEachStation(false, stationsDest);
            }

            durationsByStation = distance.getListOfDurationsPerStation(stationsCoordinates, destinationCoordinates, "foot-walking");
            int indexDest = durationsByStation.IndexOf(durationsByStation.Min());

            while (proxyCache.isAStandAvailableInStation(JsonSerializer.Serialize(stationsDest[indexDest])) == false)
            {
                durationsByStation[indexDest] = durationsByStation.Max();
                indexDest = durationsByStation.IndexOf(durationsByStation.Min());
            }           
                        

            string originString = originCoordinates[0].ToString().Replace(',', '.') + "," + originCoordinates[1].ToString().Replace(',', '.');
            string destString = destinationCoordinates[0].ToString().Replace(',', '.') + "," + destinationCoordinates[1].ToString().Replace(',', '.');

            Itineraire itineraire = new Itineraire(originString, destString, stationsOrigin[indexOrigin], stationsDest[indexDest]);
            List<Directions> directionsByBike = itineraire.GetDirections();
            List<Directions> directionsByWalk = itineraire.getItineraryFromOriginToDestinationByWalk();
            double durationTotalByBike = itineraire.getDuration(directionsByBike);
            double durationByWalk = itineraire.getDuration(directionsByWalk);

            Console.WriteLine("Duration by bike : " + durationTotalByBike + ", Duration by walk : " + durationByWalk);

            if (durationTotalByBike > durationByWalk)
            {
                activemq.sendMessage("You can go by walk, it will be quicker");
                activemq.sendMessages(getInstructions(directionsByWalk[0]));
            }
            else
            {
                activemq.sendMessage("The closest departure station is :" + stationsOrigin[indexOrigin].ToString());

                activemq.sendMessages(getInstructions(directionsByBike[0]));

                activemq.sendMessage("Vous pouvez récupérer un vélo à la station. Il y a de la route jusqu'à la prochaine station : " + stationsDest[indexDest].ToString());

                activemq.sendMessages(getInstructions(directionsByBike[1]));

                activemq.sendMessage("Laissez votre vélo ici et finissez le chemin à pied :");

                activemq.sendMessages(getInstructions(directionsByBike[2]));
            }
            

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


            foreach (Station station in stations)
            {
                double[] stationCoordinates = { station.position.longitude, station.position.latitude };
                stationsCoordinates.Add(stationCoordinates);
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

        private List<String> getInstructions(Directions directions)
        {
            List<string> instructions = new List<string>();
            foreach (Steps step in directions.features[0].properties.segments[0].steps)
            {
                instructions.Add(step.instruction + " on " + step.distance + "m");
            }
            return instructions;
        }
    }    
}
