import com.client.IServiceLetsGoBiking;
import com.client.ObjectFactory;
import com.client.ServiceLetsGoBiking;
import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.util.List;
import java.util.Scanner;

import org.apache.activemq.ActiveMQConnectionFactory;

import javax.jms.Connection;
import javax.jms.DeliveryMode;
import javax.jms.Destination;
import javax.jms.ExceptionListener;
import javax.jms.JMSException;
import javax.jms.Message;
import javax.jms.MessageConsumer;
import javax.jms.MessageProducer;
import javax.jms.Session;
import javax.jms.TextMessage;

public class Main {
    static ServiceLetsGoBiking service = new ServiceLetsGoBiking();
    static IServiceLetsGoBiking client = service.getBasicHttpBindingIServiceLetsGoBiking();

    public static void main(String[] args) {

        Scanner scanner = new Scanner(System.in);
        System.out.println("Adresse de départ :");
        String origin = scanner.nextLine();
        System.out.println("Adresse d'arrivée :");
        String destination = scanner.nextLine();

        String nbInstructionsString = client.getItinerary(origin, destination);
        Integer nbInstructions = Integer.parseInt(nbInstructionsString);

        try {
            ActiveMQConnectionFactory factory = new ActiveMQConnectionFactory("tcp://localhost:61616");

            //Create Connection
            Connection connection = factory.createConnection();

            // Start the connection
            connection.start();

            // Create Session
            Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);

            //Create queue
            Destination queue = session.createQueue("test");

            MessageConsumer consumer = session.createConsumer(queue);

            for(int i=0; i < nbInstructions; i++){
                Message message = consumer.receive();

                if (message instanceof TextMessage) {
                    TextMessage textMessage = (TextMessage) message;
                    String text = textMessage.getText();
                    System.out.println("Consumer Received: " + text);
                }
            }

            session.close();
            connection.close();
        }
        catch (Exception ex) {
            System.out.println("Exception Occured");
        }

    
    }
}
