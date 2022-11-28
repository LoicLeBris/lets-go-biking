import com.client.IServiceLetsGoBiking;
import com.client.ServiceLetsGoBiking;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.util.List;
import java.util.Scanner;

public class Main {
    static ServiceLetsGoBiking service = new ServiceLetsGoBiking();
    static IServiceLetsGoBiking client = service.getBasicHttpBindingIServiceLetsGoBiking();

    public static void main(String[] args) {

        Scanner scanner = new Scanner(System.in);
        System.out.println("Adresse de départ :");
        String origin = scanner.nextLine();
        System.out.println("Adresse d'arrivée :");
        String destination = scanner.nextLine();

        String response = client.getItinerary(origin, destination);

        try {
            List<String> instructions = new ObjectMapper().readValue(response,List.class);
            for(String instruction : instructions){
                System.out.print(instruction);
                scanner.nextLine();
            }
        } catch (JsonProcessingException e) {
            e.printStackTrace();
        }
    }
}
