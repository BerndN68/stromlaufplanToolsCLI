using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using OfficeOpenXml;
using stromlaufplanToolsCLI.Configuration;
using stromlaufplanToolsCLI.Stromlaufplan.Models;
using stromlaufplanToolsCLI.Wago;
using Project = stromlaufplanToolsCLI.Wago.Project;


namespace stromlaufplanToolsCLI.Commands
{
    internal class ExportWagoXmlCommand : CommandBase
    {
        private readonly IEnumerable<string> _ids;
        private readonly string _outputFileName;
        private readonly string _tragschienenKonfiguration;
        private readonly NameValueCollection _reihenklemmenCfg;
        private readonly ReihenklemmenCreator _reihenklemmenCreator;

        public ExportWagoXmlCommand(
            string token,
            IEnumerable<string> ids,
            string outputFileName,
            string tragschienenKonfiguration,
            LeitungstypConfigurationElementCollection configLeitungstypConfigurations,
            NameValueCollection reihenklemmenCfg)
            : base(token)
        {
            _ids = ids;
            _outputFileName = outputFileName;
            _tragschienenKonfiguration = tragschienenKonfiguration;
            _reihenklemmenCreator = new ReihenklemmenCreator(configLeitungstypConfigurations);
            _reihenklemmenCfg = reihenklemmenCfg;
        }


        public override void Execute()
        {
            Console.WriteLine("Wago SmartDesigner XML Datei exportieren");

            // Ausgabeverzeichnis erstellen
            var outputPath = Path.GetDirectoryName(_outputFileName);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var xlPackage = new ExcelPackage())
            {
                foreach (var projectId in _ids)
                {

                    Console.Write($"Projektdaten für Projekt {projectId} von stromlaufplan.de lesen ... ");
                    var data = RestClient.GetData(projectId);
                    Console.WriteLine("ok");

                    var tragschienenKonfiguration = new List<string[]>();

                    var alleKlemmleisten = data.treeNodeDatas.Values.OfType<TreeNodeDataOut>()
                        .Select(x => x.klemmleiste)
                        .GroupBy(x => x).Select(x => x.Key).ToList();

                    // tragschienenkonfiguration auswerten
                    var konfigurationenAlleProjekte = _tragschienenKonfiguration?.Split('|') ?? new string[]{};
                    foreach (var konfigurationProjekt in konfigurationenAlleProjekte)
                    {
                        var array = konfigurationProjekt.Split(':');
                        if (array[0] == projectId)
                        {
                            // gefundene Konfiguration weiter zerlegen
                            var tragschienenArray = array[1].Split(';');
                            foreach (var tragschiene in tragschienenArray)
                            {
                                tragschienenKonfiguration.Add(tragschiene.Split(','));
                            }
                        }
                    }

                    // alle noch fehlenden Klemmeleisten zu einer neuen Tragschiene hinzufügen
                    var fehlendeKlemmleisten =
                        alleKlemmleisten.Except(tragschienenKonfiguration.SelectMany(x => x)).ToList();
                    if (fehlendeKlemmleisten.Any())
                    {
                        tragschienenKonfiguration.Add(fehlendeKlemmleisten.ToArray());
                    }

                    int no = 1;
                    foreach (var einzelneTragschieneKfg in tragschienenKonfiguration)
                    {
                        Console.Write($"WAGO Klemmenplan #{no} ({string.Join(",", einzelneTragschieneKfg)}) erstellen ... ");
                        var xmlFileName = Path.Combine(outputPath, data.documentData.projectName + "_#" + no + ".xml");
                        WriteWagoXmlFile(data, xmlFileName, einzelneTragschieneKfg.ToList());
                        Console.WriteLine("ok");

                        no++;
                    }

                }
            }
        }


        private void WriteWagoXmlFile(PlanData data, string xmlFileName, List<string> tragschienenKonfiguration)
        {
            // jedes Listenelement entspricht einer


            var xmlProject = new Project { Name = data.documentData.projectAddress.Replace("\n", "- ") };

            // Tragschiene
            xmlProject.Carrier = new Carrier { ArticleNo = "210-112", Name = data.documentData.projectName };


            var outNodes = data.treeNodeDatas.Values.OfType<TreeNodeDataOut>().ToList();

            string letzteKlemmleiste = string.Empty;
            string klemmeKlemmleistenEnde = string.Empty; // Klemme die am Ende einer Klemmleiste hinzugefügt werden muss
            string klemmeMarking = string.Empty;
            int position = 1;

            foreach (var nodeData in outNodes
                .Where(x => tragschienenKonfiguration.Contains(x.klemmleiste))      // nur die angegebenen Klemmleisten exportieren
                                                                                    //.OrderBy( x => tragschienenKonfiguration.IndexOf(x.klemmleiste))       // die Reihenfolge wird durch die Konfiguration definiert.
                .OrderBy(x => x.KlemmleisteNummer)
                .ThenBy(x => x.klemmenBlockNummer))
            {
                if (letzteKlemmleiste != nodeData.klemmleiste)
                {
                    if (!string.IsNullOrEmpty(klemmeKlemmleistenEnde))
                    {
                        AddWagoKlemme(xmlProject, ref position, klemmeKlemmleistenEnde);
                        klemmeKlemmleistenEnde = string.Empty;
                    }

                    // für 230V Klemmleisten werden Einspeiseklemme und Sammelschienenträger hinzugefügt
                    if (nodeData.Type == "out")
                    {
                        // Einspeiseklemme sofort hinzufügen
                        AddWagoKlemme(xmlProject, ref position, _reihenklemmenCfg["Einspeiseklemme"], nodeData.klemmleiste);

                        // Sammelschienenträger merken wir uns bis zum Ende der Klemmleiste
                        klemmeKlemmleistenEnde = _reihenklemmenCfg["Sammelschienenträger"];
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

                    AddWagoKlemme(xmlProject, ref position, currentKlemme.ArticleNo, klemmeMarking);
                    klemmeMarking = string.Empty;
                }

            }

            // evtl. fehlt noch der Sammelschienenträger 
            if (!string.IsNullOrEmpty(klemmeKlemmleistenEnde))
            {
                AddWagoKlemme(xmlProject, ref position, klemmeKlemmleistenEnde);
            }

            var wagoXmlCae = new WagoXmlCae { Project = xmlProject };
            SaveWagoXmlFile(wagoXmlCae, xmlFileName);
        }

        private void AddWagoKlemme(Project xmlProject, ref int position, string articleNo, string marking = null)
        {
            var article = new Article { ArticleNo = articleNo, Position = position++ };
            if (!string.IsNullOrEmpty(marking))
            {
                article.Marking = new Marking();
                article.Marking.Lines.Add(marking);
            }

            xmlProject.Carrier.ProjectArticles.Add(article);
        }

        private void SaveWagoXmlFile(WagoXmlCae xml, string fileName)
        {
            try
            {
                var xmlDocument = new XmlDocument();
                var serializer = new XmlSerializer(xml.GetType());
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, xml);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }


    }
}