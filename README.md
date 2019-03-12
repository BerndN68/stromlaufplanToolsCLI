# stromlaufplanToolsCLI
Kommandozeilentool zum extrahieren von Daten aus <http://Stromlaufplan.de>

Folgende Funktionen werden im Augenblick unterstützt:
- Auslesen aller Projekte
- Erstellung einer Leitungsliste und Export in eine Excel-Datei
- Erstellung von Klemmplänen und Export in eine Excel-Datei
- Erstellung einer Datenaustauschdatei für den Wago SmartDesigner

Für die horizontalen und vertikalen Klemmenpläne wird in der Excel-Datei jeweils ein Arbeitsblatt angelegt. Die Klemmen Statistik befindet sich am Ende des vertikalen Klemmenplans.

Horizontaler Klemmenplan
<img alt="Horizontaler Klemmenplan" src="/doc/images/Klemmenplan-Horizontal.png" width="700">

Vertikaler Klemmenplan<br>
<img alt="Vertikaler Klemmenplan" src="/doc/images/Klemmenplan-Vertikal.png" width="700">

Klemmen Statistik<br>
<img alt="Klemmenplan Statistik" src="/doc/images/Klemmenplan-Statistik.png" width="400">

### Kommandozeilenparameter
-t, --token         WEBAPI Token für Authentifizierung<br>
-l, --List          Befehl zum Auslesen einer Liste mit allen Projekten<br>
-k, --Klemmenplan   Befehl zum Export eines Klemmenplans für die angegebenen Projekte<br>
-i, --ids           Liste der Projekte für die ein Klemmenplan erstellt werden soll<br>
-o, --output        Name der Excel-Datei für den Klemmenplan Export<br>
-w, --WagoXML       Befehl zum Export eines Klemmenplans zum Import im Wago SmartDesigner
-s, --tragschienen  Definiert welche Klemmeleisten zu einer Tragschiene hinzugefügt wird

Beispielaufruf für die Erstellung einer Klemmenliste für 2 stromlaufplan.de Projekte:<br>
-t <token> -l -k -o c:\temp\Klemmenplan.xlsx -i <project id#1> <project id#2>


Beispielaufruf für die Esrstellung der Datenaustauschdatei für den Wago SmartDesigner:<br>
-t <token> -o c:\temp\Stromlaufplan\Klemmenplan.xlsx -i 24073 8716 8715  -w -s 24073:X1,X2,X3,X4,X5,X15,X17;X6,X7,X8,X9,X10,X11,X20,X21,X22,X23,X25




### Konfiguration
In der config-Datei können für die unterschiedlichen Leitungstypen die Klemmen definiert werden, die im Klemmenplan verwendet werden sollen. Es kann eine Liste mit Leitungstypen definiert werden. Anhand der Kriterien typ, leitungstyp und min/max-Querschnitt wird dann der korrekte Leitungstyp mit den zugehörigen Klemmen ermittelt. Wichtig: diese Liste wird in der Reihenfolge durchsucht, wie sie in der config-Datei definiert ist.

| Kriterium     | Optional?     | Beschreibung | 
| ------------- | ------------- |------------- |
| typ | Ja | Schränkt einen Konfigurationeintrag für einen bestimmten Elementbereich ein<br>out = 230/400V Netz<br>knx_out = KNX<br>kleinsp_out = 24V Kleinspannung  |
| leitungstyp | Ja | Filter für den im Stromlaufplan angegebenen Leitungstyp, Anzahl Adern und Querschnitt<br>Es können mehrere durch Komma getrennte Filterbedingungen angegeben werden.|
| minQ | Nein | Min. Querschnitt für den ein Konfigurationseintrag gültig ist |
| maxQ | Nein | Max. Querschnitt für den ein Konfigurationseintrag gültig ist |

Wird für eine im Stromlaufplan definierte Leitung kein Leitungstyp gefunden, so wird der Export abgebrochen.

Wurde für eine Leitung ein Konfigurationseintrag gefunden, so werden die dort definierten Klemmen für die Leitung verwendet. Für jede definierte Klemme wird in der Reihenfolge der Definition versucht, entsprechende Adern in der Leitung zu finden. Welche Klemme für welche Adern verwendet wird, kann über die Eigenschaft typ definiert werden.

Folgende Typen können im Stromlaufplan in den einzelnen Bereichen verwendet werden:<br>
230/400V: N, out, PE, reserve<br>
KNX-Komponenten: out_knx+, out_knx, -out_30V+, out_30V-, out_ks+, out_ks-<br>
Kleinspannung: out_ks+, out_ks-<br>

Beispiel:
Im Stromlaufplan wird für jede Ader ein Typ definiert (L, N, PE, ...). Anhand dieses Typs werden die benötigten Klemmen ermittelt.

Eine Klemme mit dem typ="N/[out]/PE" wird verwendet, wenn eine Leitung die Adern N, L und PE besitzt.<br>
Die Klemme typ="out/[out]" wird verwendet, wenn mindestens eine Ader L vorhanden ist.

Wird in der Klemmenkonfiguration der typ "*" verwendet, dann wird die Klemme bei allen Adertypen verwendet.


Beispielkonfiguration:
```
  <Leitungstypen>
    <add typ="out"
         leitungstyp="NYM,NYY"
         minQ="0"
         maxQ="4" >
      <Klemmen>
        <add typ="N/[out]/PE" shortName="N/L/PE" color="LightSkyBlue" width="1" description="Wago - NT/L/PE - 4mm²" />
        <add typ="out/[out]" shortName="L/L" color="LightGray" width="1" description="Wago - L/L - 4mm²" />
      </Klemmen>
    </add>

    <add typ="out"
         leitungstyp="NYM,NYY"
         minQ="6"
         maxQ="16" >
      <Klemmen>
        <add typ="N/[out]/PE" shortName="N/L/PE" color="LightSkyBlue" width="2" description="Wago - NT/L/PE - 16mm²" />
        <add typ="out/[out]" shortName="L/L" color="LightGray" width="2" description="Wago - L/L - 16mm²" />
      </Klemmen>
    </add>

    <add typ="knx_out"
         leitungstyp="EIB-Y(ST)Y"
         minQ="0"
         maxQ="2.5" >
      <Klemmen>
        <add typ="out_knx+/out_knx-" shortName="K/K" color="Red" width="1" description="PHOENIX 3214663 PTTBS" />
        <add typ="out_30V+/out_30V-" shortName="K/K" color="Yellow" width="1" description="PHOENIX 3214663 PTTBS" />
        <add typ="out_ks+/out_ks-" shortName="K/K" color="Yellow" width="1" description="PHOENIX 3214663 PTTBS" />
      </Klemmen>
    </add>

    <add typ="kleinsp_out"
         leitungstyp="NYM(ST)-J 5x,Li (St) 2x,Li (St) 5x"
         minQ="0"
         maxQ="2.5" >
      <Klemmen>
        <!-- Nur Dreistockklemmen, da Schirmung auf auf Klemme aufgelegt wird -->
        <add typ="*/*/*" shortName="K/K/K" color="LightGray" width="1" description="Wago Dreistockklemme L/L/L" />
      </Klemmen>
    </add>

    <add typ="kleinsp_out"
         leitungstyp="J-Y(ST)Y"
         minQ="0"
         maxQ="2.5" >
      <Klemmen>
        <add typ="*/*/*" shortName="K/K/K" color="LightGray" width="1" description="Wago Mehrstockklemme L/L/L" />
        <add typ="*/*" shortName="K/K" color="LightGray" width="1" description="Wago Mehrstockklemme L/L" />
      </Klemmen>
    </add>    

    <add typ="kleinsp_out"
         leitungstyp="NYM-J"
         minQ="0"
         maxQ="2.5" >
      <Klemmen>
        <add typ="*/*/*" shortName="K/K/K" color="LightGray" width="1" description="Wago Mehrstockklemme L/L/L" />
        <add typ="*/*" shortName="K/K" color="LightGray" width="1" description="Wago Mehrstockklemme L/L" />
      </Klemmen>
    </add>    

    <add typ=""
         leitungstyp="(intern)"
         minQ="0"
         maxQ="2.5"
         ignore="true">
      <Klemmen>
      </Klemmen>
    </add>

  </Leitungstypen>
```




