using System.Collections.Generic;
using CommandLine;

namespace stromlaufplanToolsCLI
{
    public class Options
    {
        [Option('t', "token", Required = true, HelpText = "WebAPI Token für Authentifizierung")]
        public string Token { get; set; }

        [Option('l', "List", Required = false, HelpText = "Liste der Projekte ausgeben")]
        public bool List { get; set; }

        [Option('k', "Klemmliste", Required = false, HelpText = "Liste der Projekte ausgeben")]
        public bool Klemmliste { get; set; }

        [Option('i', "ids", Required = false, HelpText = "Projekte die verarbeitet werden sollen")]
        public IEnumerable<string> Ids { get; set; }


        [Option('o', "output", Required = true, HelpText = "Dateiname für Ausgabedatei")]
        public string OutputFilename { get; set; }

    }
}