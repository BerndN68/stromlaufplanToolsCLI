using System.Collections.Generic;
using CommandLine;

namespace stromlaufplanToolsCLI
{
    public class Options
    {
        [Option('t', "token", Required = true, HelpText = "WebAPI Token für Authentifizierung")]
        public string Token { get; set; }

        [Option('l', "List", Required = false, HelpText = "Befehl zum Auslesen einer Liste mit allen Projekten")]
        public bool List { get; set; }

        [Option('k', "Klemmenplan", Required = false, HelpText = "Befehl zum Export eines Klemmenplans für die angegebenen Projekte")]
        public bool Klemmenplan { get; set; }

        [Option('i', "ids", Required = false, HelpText = "Liste der Projekte für die ein Klemmenplan erstellt werden soll")]
        public IEnumerable<string> Ids { get; set; }

        [Option('o', "output", Required = false, HelpText = "Verzeichnis in das die Daten exportiert werden")]
        public string OutputPath { get; set; }

        [Option('w', "", Required = false, HelpText = "Befehl zum Export eines Klemmenplans zum Import im Wago SmartDesigner")]
        public bool WagoXml { get; set; }

        [Option('c', "ClipProject", Required = false, HelpText = "Befehl zum Export eines Klemmenplans zum Import im Phoenix Contact Clip Project")]
        public bool ClipProject { get; set; }

        [Option('s', "tragschienen", Required = false, HelpText = "Definiert welche Klemmeleisten zu einer Tragschiene hinzugefügt wird")]
        public string TragschienenKonfiguration { get; set; }

        [Option('p', "Producer", Required = false,
            HelpText = "Definiert den Hersteller der Klemmen (Phoenix oder Wago)")]
        public Producer Producer { get; set; } = Producer.Wago;
    }
}