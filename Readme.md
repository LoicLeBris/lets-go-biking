# Projet Let's Go Biking
##### _Service Oriented Computing_
###### Loïc Le Bris et Thomas Paul

Dans le cadre du cours de SOC, nous avons créé un projet nommé Let's Go Biking.
Il s'agit d'un projet permettant de récupérer les informations sur un itinéraire allant d'un point d'origine à une destination fournis par l'utilisateur. Le trajet renvoie alors le meilleur itinéraire, utilisant ou non les vélos mis à dispositions par l'entreprise JCDecaux.
Ce projet a pour but de manipuler les appels à différentes api, créer des serveurs SOAP et un client qui les utilise.

## Comment lancer le programme ?

Pour lancer le programme, il suffit de lancer le fichier script.bat que vous trouverez à la racine du projet. Le script lancera alors automatiquement les deux serveurs (Proxy et Routing), activemq (si cela ne fonctionne pas, ajoutez le chemin vers activemq dans vos variables d'environnement 😉) ainsi que le client Java dans lequel vous pourrez rentrer les informations nécessaires au programme. Si le programme vous demande de passer en administrateur, ne vous inquietez pas c'est normal, cela permet d'assurer le fonctionnement du Routing Server.


**Command lines**
```
cd <-- the directory where the project is -->
./script.bat
```