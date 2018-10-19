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


        [Option('o', "output", Required = true, HelpText = "Name der Excel-Datei für den Klemmenplan Export")]
        public string OutputFilename { get; set; }

    }
}