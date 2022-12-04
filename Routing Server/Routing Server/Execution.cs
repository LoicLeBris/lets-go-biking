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
            //int stationIndex = distance.getShortestDistance(stationsCoordinates, originCoordinates, "foot-walking");

            List<double> durationsByStation = distance.getListOfDurationsPerStation(stationsCoordinates, originCoordinates, "foot-walking");
            int indexOrigin = durationsByStation.IndexOf(durationsByStation.Min());

            while (proxyCache.isABikeAvailableInStation(JsonSerializer.Serialize(stationsOrigin[indexOrigin])) == false)
            {
                durationsByStation[indexOrigin] = durationsByStation.Max();                
                indexOrigin = durationsByStation.IndexOf(durationsByStation.Min());
            }

            instructions.Add("Origin Station:" + stationsOrigin[indexOrigin]);

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

            instructions.Add("Destination Station:" + stationsDest[indexDest]);

            instructions.Add("La station la plus proche est :" + stationsOrigin[indexOrigin].ToString());

            string originString = originCoordinates[0].ToString().Replace(',', '.') + "," + originCoordinates[1].ToString().Replace(',', '.');
            string destString = destinationCoordinates[0].ToString().Replace(',', '.') + "," + destinationCoordinates[1].ToString().Replace(',', '.');

            Itineraire itineraire = new Itineraire(originString, destString, stationsOrigin[indexOrigin], stationsDest[indexDest]);

            addInstructions(instructions, itineraire.getItineraryToDepartureStation());

            instructions.Add("Vous pouvez récupérer un vélo à la station. Il y a de la route jusqu'à la prochaine station : " + stationsDest[indexDest].ToString());

            addInstructions(instructions, itineraire.getItineraryToArrivalStation());

            instructions.Add("Laissez votre vélo ici et finissez le chemin à pied :");

            addInstructions(instructions, itineraire.getItineraryToDestinationAdress());

            ActiveMq activemq = new ActiveMq();
            activemq.lauchActiveMq(instructions);

            return instructions.Count.ToString();
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
    }    
}
