import com.client.IServiceLetsGoBiking;
import com.client.ServiceLetsGoBiking;

import java.util.Scanner;

import com.sun.xml.ws.fault.ServerSOAPFaultException;
import org.apache.activemq.ActiveMQConnectionFactory;

import javax.jms.Connection;
import javax.jms.Destination;
import javax.jms.JMSException;
import javax.jms.Message;
import javax.jms.MessageConsumer;
import javax.jms.MessageListener;
import javax.jms.Session;
import javax.jms.TextMessage;

public class Main implements MessageListener {
    static ServiceLetsGoBiking service = new ServiceLetsGoBiking();
    static IServiceLetsGoBiking client = service.getBasicHttpBindingIServiceLetsGoBiking();
    static MessageConsumer consumer;
    static Connection connection;
    static Session session;

    public static void main(String[] args) {

        try{
            Scanner scanner = new Scanner(System.in);
            System.out.println("Adresse de départ :");
            String origin = scanner.nextLine();
            System.out.println("Adresse d'arrivée :");
            String destination = scanner.nextLine();

            new Main().launchActiveMq();

            client.getItinerary(origin, destination);

            scanner.nextLine();

            System.out.println("Do you want to enter another itinerary ? (y/n)");
            if(scanner.nextLine().equals("y")){
                main(null);
            }
            else{
                session.close();
                connection.close();
            }
        } catch (ServerSOAPFaultException | JMSException e) {
            System.out.println(e.getMessage());
            main(null);
        }
    }

    public void launchActiveMq(){
        try {
            ActiveMQConnectionFactory factory = new ActiveMQConnectionFactory("tcp://localhost:61616");

            //Create Connection
            connection = factory.createConnection();

            // Start the connection
            connection.start();

            // Create Session
            session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);

            //Create queue
            Destination queue = session.createQueue("test");

            consumer = session.createConsumer(queue);
            
            consumer.setMessageListener(this);
        }
        catch (Exception ex) {
            System.out.println("Exception Occured");
        }
    }

    @Override
    public void onMessage(Message message) {
        try{
            if (message instanceof TextMessage) {
                TextMessage textMessage = (TextMessage) message;
                String text = textMessage.getText();
                System.out.println(text);
            }
        } catch(java.lang.RuntimeException | JMSException e){
            System.out.println("Error " + e);
        }
    }
}
