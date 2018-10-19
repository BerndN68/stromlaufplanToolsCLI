# stromlaufplanToolsCLI
Kommandozeilentool zum extrahieren von Daten aus Stromlaufplan.de

Folgende Funktionen werden im Augenblick unterstützt:
- Auslesen aller Projekte
- Erstellung einer Leitungsliste
- Erstellung von Klemmplänen


Kommandozeilenparameter:
-t, --token         WEBAPI Token für Authentifizierung
-l, --List          Befehl zum Auslesen einer Liste mit allen Projekten
-k, --Klemmenplan   Befehl zum Export eines Klemmenplans für die angegebenen Projekte
-i, --ids           Liste der Projekte für die ein Klemmenplan erstellt werden soll
-o, --output        Name der Excel-Datei für den Klemmenplan Export

Beispielaufruf für die Erstellung einer Klemmenliste für 2 stromlaufplan.de Projekte:
-t <token> -l -k -o c:\temp\Klemmenplan.xlsx -i <project id#1> 24073 8715

Konfiguration:
In der config-Datei können für die unterschiedlichen Leitungstypen die zu Klemmen definiert werden die im KLemmenplan verwendet werden können.

Anhand des im Stromlaufplan verwendeten Leitungstyps und des Querschnittes werden die konfigurierten Klemmen berücksichtigt.
