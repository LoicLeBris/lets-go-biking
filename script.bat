START "" ".\ProxyCache\ProxyCache\bin\Debug\ProxyCache.exe"
START "" ".\Routing Server\Routing Server\bin\Debug\Routing Server.exe"
START activemq start
cd HeavyClient && mvn clean package && mvn exec:java