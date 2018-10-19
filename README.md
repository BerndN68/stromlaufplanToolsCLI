# stromlaufplanToolsCLI
Kommandozeilentool zum extrahieren von Daten aus <http://Stromlaufplan.de>

Folgende Funktionen werden im Augenblick unterstützt:
- Auslesen aller Projekte
- Erstellung einer Leitungsliste
- Erstellung von Klemmplänen


<h3>Kommandozeilenparameter</h3><br>
-t, --token         WEBAPI Token für Authentifizierung<br>
-l, --List          Befehl zum Auslesen einer Liste mit allen Projekten<br>
-k, --Klemmenplan   Befehl zum Export eines Klemmenplans für die angegebenen Projekte<br>
-i, --ids           Liste der Projekte für die ein Klemmenplan erstellt werden soll<br>
-o, --output        Name der Excel-Datei für den Klemmenplan Export<br>

Beispielaufruf für die Erstellung einer Klemmenliste für 2 stromlaufplan.de Projekte:<br>
-t <token> -l -k -o c:\temp\Klemmenplan.xlsx -i <project id#1> 24073 8715

<h3>Konfiguration</h3><br>
In der config-Datei können für die unterschiedlichen Leitungstypen die zu Klemmen definiert werden die im KLemmenplan verwendet werden können.

Anhand des im Stromlaufplan verwendeten Leitungstyps und des Querschnittes werden die konfigurierten Klemmen berücksichtigt.
