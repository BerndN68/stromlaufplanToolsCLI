using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using CsvHelper;
using OfficeOpenXml;
using stromlaufplanToolsCLI.Configuration;
using stromlaufplanToolsCLI.Export.Phoenix;
using stromlaufplanToolsCLI.Export.Phoenix.Extras;
using stromlaufplanToolsCLI.Stromlaufplan.Models;


namespace stromlaufplanToolsCLI.Commands
{
    internal class ExportPhoenixClipProjectCsvCommand : ExportDataCommandBase
    {

        public ExportPhoenixClipProjectCsvCommand(
            string token,
            IEnumerable<string> ids,
            string outputPath,
            string tragschienenKonfiguration,
            LeitungstypConfigurationElementCollection configLeitungstypConfigurations,
            NameValueCollection reihenklemmenCfg)
            : base(token, ids, outputPath, tragschienenKonfiguration, configLeitungstypConfigurations, reihenklemmenCfg)
        {
        }


        public override void Execute()
        {
            Console.WriteLine("Clip Project CSV Datei exportieren");

            // Ausgabeverzeichnis erstellen
            EnsureOutputPathExists();

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            foreach (var projectId in _ids)
            {

                Console.Write($"Projektdaten für Projekt {projectId} von stromlaufplan.de lesen ... ");
                var data = RestClient.GetData(projectId);
                Console.WriteLine("ok");


                var alleKlemmleisten = data.treeNodeDatas.Values.OfType<TreeNodeDataOut>()
                    .Select(x => x.klemmleiste)
                    .GroupBy(x => x).Select(x => x.Key).ToList();


                var tragschienenKonfiguration = new List<TrageschieneKonfiguration>();
                if (!string.IsNullOrEmpty(_tragschienenKonfiguration))
                {
                    // tragschienenkonfiguration auswerten
                    tragschienenKonfiguration = CreateTrageschienenKonfigurationFromKonfiguration(projectId, alleKlemmleisten);
                }
                else
                {
                    tragschienenKonfiguration =
                        CreateEineTrageschieneKonfigurationProBlock(projectId, alleKlemmleisten);
                }
                

                // alle Trageschienen werden in eine Datei exportiert
                var csvFilePath = Path.Combine(_outputPath, $"ClipProject_{data.documentData.projectName}.csv");

                using (var writer = new StreamWriter(csvFilePath))
                {
                    using (var csv = new CsvWriter(writer))
                    {
                        //csv.Configuration.QuoteAllFields = false;
                        csv.Configuration.ShouldQuote = (a, b) => false;
                        csv.Configuration.Delimiter = ";";
                        csv.Configuration.HasHeaderRecord = false;


                        int lineNo = 1;
                        foreach (var einzelneTragschieneKfg in tragschienenKonfiguration)
                        {
                            Console.Write($"Klemmenplan #{lineNo} ({string.Join(",", einzelneTragschieneKfg)}) erstellen ... ");

                            CreateExportData(csv, data, einzelneTragschieneKfg, ref lineNo);

                            Console.WriteLine("ok");
                        }
                    }
                }
            }
        }




        private void CreateExportData(CsvWriter csv,PlanData data, TrageschieneKonfiguration tragschienenKonfiguration, ref int lineNo)
        {
            var outNodes = data.treeNodeDatas.Values.OfType<TreeNodeDataOut>().ToList();
            var nodesToExport = outNodes
                .Where(x => tragschienenKonfiguration.Klemmleisten
                    .Contains(x.klemmleiste)) // nur die angegebenen Klemmleisten exportieren
                //.OrderBy( x => tragschienenKonfiguration.IndexOf(x.klemmleiste))       // die Reihenfolge wird durch die Konfiguration definiert.
                .OrderBy(x => x.KlemmleisteNummer)
                .ThenBy(x => x.klemmenBlockNummer)
                .ToList();

            if (!nodesToExport.Any())
            {
                return;
            }


            WriteTrageschieneToCsv(csv, ref lineNo, data, tragschienenKonfiguration);
            WriteFreeSpacingToCsv(csv, ref lineNo);


            string letzteKlemmleiste = string.Empty;
            ReihenklemmeInfo klemmeKlemmleistenEnde = null; // Klemme die am Ende einer Klemmleiste hinzugefügt werden muss
            string klemmeMarking = string.Empty;
            int position = 1;

            var terminals = new List<Terminal>();
            foreach (var nodeData in nodesToExport)
            {
                if (letzteKlemmleiste != nodeData.klemmleiste)
                {
                    WriteKlemmleisteToCsv(csv, ref lineNo, terminals, klemmeKlemmleistenEnde);

                    terminals = new List<Terminal>();

                    // für 230V Klemmleisten werden Einspeiseklemme und Sammelschienenträger hinzugefügt
                    if (nodeData.Type == "out")
                    {
                        // Einspeiseklemme sofort hinzufügen
                        AddPhoenixKlemme(terminals, ref lineNo, CreateKlemmeFromConfig(_reihenklemmenCfg["PhoenixEinspeiseklemme"])); // _reihenklemmenCfg["Einspeiseklemme"]);

                        // Sammelschienenträger merken wir uns bis zum Ende der Klemmleiste
                        klemmeKlemmleistenEnde = CreateKlemmeFromConfig(_reihenklemmenCfg["PhoenixSammelschienenträger"]); //reihenklemmenCfg["Sammelschienenträger"];
                    }
                    else
                    {
                        // ansonsten beschriften wir die erste Klemme mit dem Namend er Klemmleiste
                        klemmeMarking = nodeData.klemmleiste;
                    }
                }
                letzteKlemmleiste = nodeData.klemmleiste;


                int currentKlemmeNrInLeiste = 0;
                var reihenklemmen = _reihenklemmenCreator.CreateReihenklemmen(nodeData, ref currentKlemmeNrInLeiste);

                if (!reihenklemmen.Any())
                {
                    continue;
                }

                for (int idx = 0; idx < reihenklemmen.Count; idx++)
                {
                    var currentKlemme = reihenklemmen[idx];

                    AddPhoenixKlemme(terminals, ref lineNo, currentKlemme);
                    klemmeMarking = string.Empty;
                }

            }

            WriteKlemmleisteToCsv(csv, ref lineNo, terminals, klemmeKlemmleistenEnde);
        }

        private void WriteKlemmleisteToCsv(CsvWriter csv, ref int lineNo, List<Terminal> terminals, ReihenklemmeInfo klemmeKlemmleistenEnde)
        {
            if (!terminals.Any())
            {
                return;
            }

            if (lineNo > 1)
            {
                WriteFreeSpacingToCsv(csv, ref lineNo);
            }


            if (klemmeKlemmleistenEnde != null)
            {
                AddPhoenixKlemme(terminals, ref lineNo, klemmeKlemmleistenEnde);
            }

            // Clipfix zum Start
            TryWriteEndBracketToCsv(csv, ref lineNo, CreateKlemmeFromConfig(_reihenklemmenCfg["PhoenixKlemmenleisteStart"]));

            csv.WriteRecords(terminals);

            // Clipfix zum Ende
            TryWriteEndBracketToCsv(csv, ref lineNo, CreateKlemmeFromConfig(_reihenklemmenCfg["PhoenixKlemmenleisteEnde"]));
        }

        private bool TryWriteEndBracketToCsv(CsvWriter csv, ref int lineNo, ReihenklemmeInfo reihenklemmeInfo)
        {
            if (string.IsNullOrEmpty(reihenklemmeInfo.ArticleNo))
            {
                return false;
            }

            csv.WriteRecord(
                new EndBracket
                {
                    LineNumber = lineNo++,
                    ArticleNumber = reihenklemmeInfo.ArticleNo,
                    ArticleName = reihenklemmeInfo.ArticleName,
                });
            csv.NextRecord();

            return true;
        }

        private void WriteFreeSpacingToCsv(CsvWriter csv, ref int lineNo, long width = 0)
        {
            var element = new FreeSpacing();

            if (width > 0)
            {
                element.Width = width;
            }

            csv.WriteRecord(element);
            csv.NextRecord();
        }

        private ReihenklemmeInfo CreateKlemmeFromConfig(string cfgKlemme)
        {
            if (string.IsNullOrEmpty(cfgKlemme))
            {
                return new ReihenklemmeInfo();
            }

            var items = cfgKlemme.Split(new string[] {"|"}, StringSplitOptions.RemoveEmptyEntries);

            return new ReihenklemmeInfo(items[0], items[1]);
        }

        private static void WriteTrageschieneToCsv(CsvWriter csv, ref int lineNo, PlanData data,
            TrageschieneKonfiguration tragschienenKonfiguration)
        {
            csv.WriteRecord(
                new Rail
                {
                    Name = $"{data.documentData.projectName} - {tragschienenKonfiguration.Name}",
                    LineNumber = lineNo++,
                    ArticleNumber = "801733",
                    ArticleName = "NS 35/7,5 PERF 2000MM",
                });
            csv.NextRecord();
        }


        private void AddPhoenixKlemme(List<Terminal> terminals, ref int lineNo, ReihenklemmeInfo klemme)
        {
            var terminal = new Terminal { LineNumber = lineNo++, ArticleNumber = klemme.ArticleNo, ArticleName = klemme.ArticleName};
            terminals.Add(terminal);
        }

    }
}