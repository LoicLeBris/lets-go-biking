
package com.client;

import java.net.MalformedURLException;
import java.net.URL;
import javax.xml.namespace.QName;
import javax.xml.ws.*;


/**
 * This class was generated by the JAX-WS RI.
 * JAX-WS RI 4.0.0-M4
 * Generated source version: 3.0
 * 
 */
@WebServiceClient(name = "ServiceLetsGoBiking", targetNamespace = "http://tempuri.org/", wsdlLocation = "http://localhost:8733/Routing_Server/ServiceLetsGoBiking/?wsdl")
public class ServiceLetsGoBiking
    extends Service
{

    private final static URL SERVICELETSGOBIKING_WSDL_LOCATION;
    private final static WebServiceException SERVICELETSGOBIKING_EXCEPTION;
    private final static QName SERVICELETSGOBIKING_QNAME = new QName("http://tempuri.org/", "ServiceLetsGoBiking");

    static {
        URL url = null;
        WebServiceException e = null;
        try {
            url = new URL("http://localhost:8733/Routing_Server/ServiceLetsGoBiking/?wsdl");
        } catch (MalformedURLException ex) {
            e = new WebServiceException(ex);
        }
        SERVICELETSGOBIKING_WSDL_LOCATION = url;
        SERVICELETSGOBIKING_EXCEPTION = e;
    }

    public ServiceLetsGoBiking() {
        super(__getWsdlLocation(), SERVICELETSGOBIKING_QNAME);
    }

    public ServiceLetsGoBiking(WebServiceFeature... features) {
        super(__getWsdlLocation(), SERVICELETSGOBIKING_QNAME, features);
    }

    public ServiceLetsGoBiking(URL wsdlLocation) {
        super(wsdlLocation, SERVICELETSGOBIKING_QNAME);
    }

    public ServiceLetsGoBiking(URL wsdlLocation, WebServiceFeature... features) {
        super(wsdlLocation, SERVICELETSGOBIKING_QNAME, features);
    }

    public ServiceLetsGoBiking(URL wsdlLocation, QName serviceName) {
        super(wsdlLocation, serviceName);
    }

    public ServiceLetsGoBiking(URL wsdlLocation, QName serviceName, WebServiceFeature... features) {
        super(wsdlLocation, serviceName, features);
    }

    /**
     * 
     * @return
     *     returns IServiceLetsGoBiking
     */
    @WebEndpoint(name = "BasicHttpBinding_IServiceLetsGoBiking")
    public IServiceLetsGoBiking getBasicHttpBindingIServiceLetsGoBiking() {
        return super.getPort(new QName("http://tempuri.org/", "BasicHttpBinding_IServiceLetsGoBiking"), IServiceLetsGoBiking.class);
    }

    /**
     * 
     * @param features
     *     A list of {@link jakarta.xml.ws.WebServiceFeature} to configure on the proxy.  Supported features not in the <code>features</code> parameter will have their default values.
     * @return
     *     returns IServiceLetsGoBiking
     */
    @WebEndpoint(name = "BasicHttpBinding_IServiceLetsGoBiking")
    public IServiceLetsGoBiking getBasicHttpBindingIServiceLetsGoBiking(WebServiceFeature... features) {
        return super.getPort(new QName("http://tempuri.org/", "BasicHttpBinding_IServiceLetsGoBiking"), IServiceLetsGoBiking.class, features);
    }

    private static URL __getWsdlLocation() {
        if (SERVICELETSGOBIKING_EXCEPTION!= null) {
            throw SERVICELETSGOBIKING_EXCEPTION;
        }
        return SERVICELETSGOBIKING_WSDL_LOCATION;
    }

}
