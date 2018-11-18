using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using stromlaufplanToolsCLI.Configuration;
using stromlaufplanToolsCLI.EPPlus;
using stromlaufplanToolsCLI.StromplanModels;
using stromlaufplanToolsCLI.Common;

namespace stromlaufplanToolsCLI.Commands
{
    public class ExportKlemmlisteCommand : CommandBase
    {
        private readonly IEnumerable<string> _ids;
        private readonly string _outputFileName;
        private readonly LeitungstypConfigurationElementCollection _configLeitungstypConfigurations;

        public ExportKlemmlisteCommand(
            string token, 
            IEnumerable<string> ids, 
            string outputFileName,
            LeitungstypConfigurationElementCollection configLeitungstypConfigurations)
            : base(token)
        {
            _ids = ids;
            _outputFileName = outputFileName;
            _configLeitungstypConfigurations = configLeitungstypConfigurations;
        }

        public override void Execute()
        {
            Console.WriteLine("Klemmlisten exportieren");

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var xlPackage = new ExcelPackage())
            {
                foreach (var projectId in _ids)
                {

                    Console.Write($"Projektdaten für Projekt {projectId} von stromlaufplan.de lesen ... ");
                    var data = ExecuteUrl<PlanData>($"projects/{projectId}/export/json");
                    Console.WriteLine("ok");

                    ExportKlemmliste(xlPackage, data);
                }

                //Save the new workbook. We haven't specified the filename so use the Save as method.
                Console.Write($"Excel Datei speichern {_outputFileName} ... ");
                xlPackage.SaveAs(new FileInfo(_outputFileName));
                Console.WriteLine("ok");
            }

        }

        private void ExportKlemmliste(ExcelPackage xlPackage, PlanData data)
        {
            var outNodes = data.treeNodeDatas.Values
                .OfType<TreeNodeDataOut>().ToList();

            //A workbook must have at least on cell, so lets add one... 
            Console.Write($"Datenliste erstellen ... ");
            var ws = xlPackage.Workbook.Worksheets.Add($"{data.documentData.projectName}-LL");
            WriteLeitungsliste(ws, outNodes);
            Console.WriteLine("ok");

            Console.Write($"Klemmenplan (Vertikal) erstellen ... ");
            ws = xlPackage.Workbook.Worksheets.Add($"{data.documentData.projectName}-KL-V");
            WriteKlemmlisteVertikal(ws, outNodes);
            Console.WriteLine("ok");

            Console.Write($"Klemmenplan (Horizontal) erstellen ... ");
            ws = xlPackage.Workbook.Worksheets.Add($"{data.documentData.projectName}-KL-H");
            WriteKlemmlisteHorizontal(ws, outNodes);
            Console.WriteLine("ok");
        }


        private void WriteKlemmlisteHorizontal(ExcelWorksheet ws, IEnumerable<TreeNodeDataOut> outNodes)
        {
            string letzteKlemmleiste = string.Empty;
            int colNo = 1;
            int currentKlemmeNrInLeiste = 0;

            const double COLUMN_WIDTH = 2.0;

            ws.Row(2).Height = 69.8;
            ws.Row(3).Height = 21;
            ws.Row(4).Height = 38.3;
            ws.Row(5).Height = 168.8;

            foreach (var nodeData in outNodes.OrderBy(x => x.KlemmleisteNummer).ThenBy(x => x.klemmenBlockNummer))
            {
                if (letzteKlemmleiste != nodeData.klemmleiste)
                {
                    currentKlemmeNrInLeiste = 0;
                }

                var reihenklemmen = CreateReihenklemmen(nodeData, ref currentKlemmeNrInLeiste);
                if (!reihenklemmen.Any())
                {
                    continue;
                }

                ExcelRange range;

                // Blocktrenner einfügen?
                if (letzteKlemmleiste != nodeData.klemmleiste)
                {
                    for (int col = colNo; col <= colNo + 2; col++)
                    {
                        ws.Column(col).SetTrueColumnWidth(COLUMN_WIDTH);
                    }

                    colNo++;

                    ws.Cells[5, colNo].Value = nodeData.klemmleiste;
                    range = ws.Cells[5, colNo, 5, colNo + 1];
                    range.Merge = true;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    range.Style.TextRotation = 90;
                    range.Style.WrapText = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.LightSkyBlue);

                    letzteKlemmleiste = nodeData.klemmleiste;
                    colNo += 2;
                }

                var leitungLastCol = colNo + reihenklemmen.Sum(x => x.Width) - 1;


                for (int col = colNo; col <= leitungLastCol; col++)
                {
                    ws.Column(col).SetTrueColumnWidth(COLUMN_WIDTH);
                }

                // Leitungstyp/Querschnitt
                ws.Cells[2, colNo].Value = $"{nodeData.leitungstyp} {nodeData.adern.Length}x{nodeData.querschnitt}";
                range = ws.Cells[2, colNo, 2, leitungLastCol];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.TextRotation = 90;

                // KlemmblockNummer
                ws.Cells[3, colNo].Value = nodeData.klemmenBlockNummer;
                range = ws.Cells[3, colNo, 3, leitungLastCol];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Klemmen
                for (int idx = 0; idx < reihenklemmen.Count; idx++)
                {
                    var col = colNo + (idx * reihenklemmen[idx].Width);
                    ws.Cells[4, col].Value = reihenklemmen[idx].Name;
                    range = ws.Cells[4, col, 4, col + reihenklemmen[idx].Width - 1];
                    range.Merge = true;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(reihenklemmen[idx].Color);
                    range.Style.TextRotation = 90;
                }

                // Leitungsname
                var name = nodeData.leitungsname;
                if (!string.IsNullOrEmpty(nodeData.leitungsnummer))
                {
                    name = $"{nodeData.leitungsnummer}{Environment.NewLine}" + name;
                }

                ws.Cells[5, colNo].Value = name;
                range = ws.Cells[5, colNo, 5, leitungLastCol];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                range.Style.TextRotation = 90;
                range.Style.WrapText = true;

                colNo = leitungLastCol + 1;
            }
        }


        private void WriteKlemmlisteVertikal(ExcelWorksheet ws, IEnumerable<TreeNodeDataOut> outNodes)
        {
            var gesamtanzahlReihenklemmen = new Dictionary<string, int>();
            var leitungstypMitQuerschnittStatistik = new Dictionary<string, int>();

            string letzteKlemmleiste = string.Empty;
            int rowNo = 1;
            int currentKlemmeNrInLeiste = 0;
            int leitungNr = 1;

            ws.Cells[rowNo, 1].Value = "Nr";
            ws.Column(1).Width = 4;

            ws.Cells[rowNo, 2].Value = "L-Nummer";
            ws.Column(2).Width = 13;

            ws.Cells[rowNo, 3].Value = "Leitungsname";
            ws.Column(3).Width = 38;

            ws.Cells[rowNo, 4].Value = "Leitungstyp";
            ws.Column(4).Width = 13.5;

            ws.Cells[rowNo, 5].Value = "Klemmenleiste";
            ws.Column(5).Width = 5;

            ws.Cells[rowNo, 6].Value = "Klemme";
            ws.Column(6).Width = 11;

            ws.Cells[rowNo, 8].Value = "Klemmentyp";
            ws.Column(8).Width = 11;

            ws.Cells[rowNo, 9].Value = "Beschreibung";
            ws.Column(9).Width = 28;

            rowNo++;

            foreach (var nodeData in outNodes.OrderBy(x => x.KlemmleisteNummer).ThenBy(x => x.klemmenBlockNummer))
            {
                if (letzteKlemmleiste != nodeData.klemmleiste)
                {
                    currentKlemmeNrInLeiste = 0;

                    // Beim Wechsel der Klemmleiste eine Leerzeile einfügen
                    if (!string.IsNullOrEmpty(letzteKlemmleiste))
                    {
                        ws.Row(rowNo).Height = 10;
                        rowNo++;
                    }
                }
                letzteKlemmleiste = nodeData.klemmleiste;


                var reihenklemmen = CreateReihenklemmen(nodeData, ref currentKlemmeNrInLeiste);

                if (!reihenklemmen.Any())
                {
                    continue;
                }

                var leitungLastRow = rowNo + reihenklemmen.Sum( x => x.Width ) - 1;

                // Col1: Nr
                ws.Cells[rowNo, 1].Value = leitungNr++;
                var range = ws.Cells[rowNo, 1, leitungLastRow, 1];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                // Col2: L-Nummer
                ws.Cells[rowNo, 2].Value = $"{nodeData.leitungsnummer}";
                range = ws.Cells[rowNo, 2, leitungLastRow, 2];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);


                // Col3: L-Nummer
                ws.Cells[rowNo, 3].Value = $"{nodeData.leitungsname}";
                range = ws.Cells[rowNo, 3, leitungLastRow, 3];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                // Col4: Leitungstyp
                ws.Cells[rowNo, 4].Value = $"{nodeData.leitungstyp} {nodeData.adern.Length}x{nodeData.querschnitt}";
                range = ws.Cells[rowNo, 4, leitungLastRow, 4];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                // Col5: Klemmenleiste
                ws.Cells[rowNo, 5].Value = $"{nodeData.klemmleiste}";
                range = ws.Cells[rowNo, 5, leitungLastRow, 5];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                // Col6: Klemmenblocknummer
                ws.Cells[rowNo, 6].Value = $"{nodeData.klemmenBlockNummer}";
                range = ws.Cells[rowNo, 6, leitungLastRow, 6];
                range.Merge = true;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                range.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                for (int idx = 0; idx < reihenklemmen.Count; idx++)
                {
                    var currentKlemme = reihenklemmen[idx];

                    if (!gesamtanzahlReihenklemmen.ContainsKey(currentKlemme.Description))
                    {
                        gesamtanzahlReihenklemmen[currentKlemme.Description] = 1;
                    }
                    else
                    {
                        gesamtanzahlReihenklemmen[currentKlemme.Description] = gesamtanzahlReihenklemmen[currentKlemme.Description] + 1;
                    }

                    // Col 7: Klemmen
                    var klemme = string.Join(",", reihenklemmen[idx].Klemmen);
                    var row = rowNo + (idx * reihenklemmen[idx].Width);

                    ws.Cells[row, 7].Value = $"{klemme}";
                    range = ws.Cells[row, 7, row + reihenklemmen[idx].Width - 1, 7];
                    range.Merge = true;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    // Col 8: Klemmentyp
                    ws.Cells[row,8].Value = reihenklemmen[idx].Name;
                    range = ws.Cells[row, 8, row + reihenklemmen[idx].Width - 1, 8];
                    range.Merge = true;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(reihenklemmen[idx].Color);

                    // Col 9: Beschreibung
                    ws.Cells[row, 9].Value = reihenklemmen[idx].Description;
                    range = ws.Cells[row, 9, row + reihenklemmen[idx].Width - 1, 98];
                    range.Merge = true;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    range.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                var leitungstyp = CreateLeitungstypWithQuerschnitt(nodeData);
                if (leitungstypMitQuerschnittStatistik.ContainsKey(leitungstyp))
                {
                    leitungstypMitQuerschnittStatistik[leitungstyp] = ++leitungstypMitQuerschnittStatistik[leitungstyp];
                }
                else
                {
                    leitungstypMitQuerschnittStatistik[leitungstyp] = 1;
                }

                rowNo = leitungLastRow + 1;
            }

            // Statistik Leitungstypen ausgeben
            rowNo += 3;
            WriteLeitungstypMitQuerschnittStatistik(ws, ref rowNo, leitungstypMitQuerschnittStatistik);

            // Statistik Klemmen ausgeben
            rowNo += 3;
            WriteKlemmenStatistik(ws, ref rowNo, gesamtanzahlReihenklemmen);

        }

        private void WriteLeitungstypMitQuerschnittStatistik(ExcelWorksheet ws, ref int rowNo, Dictionary<string, int> leitungstypMitQuerschnittStatistik)
        {
            ws.Cells[rowNo, 3].Value = "Leitungstyp";
            ws.Cells[rowNo, 4].Value = "Anzahl";

            foreach (var reihenklemme in leitungstypMitQuerschnittStatistik.OrderBy( x => x.Key))
            {
                rowNo++;
                ws.Cells[rowNo, 3].Value = reihenklemme.Key;
                ws.Cells[rowNo, 4].Value = reihenklemme.Value;
            }
        }

        private void WriteKlemmenStatistik(ExcelWorksheet ws, ref int rowNo, Dictionary<string, int> gesamtanzahlReihenklemmen)
        {
            ws.Cells[rowNo, 3].Value = "Klemme";
            ws.Cells[rowNo, 4].Value = "Anzahl";

            foreach (var reihenklemme in gesamtanzahlReihenklemmen.OrderBy(x => x.Key))
            {
                rowNo++;
                ws.Cells[rowNo, 3].Value = reihenklemme.Key;
                ws.Cells[rowNo, 4].Value = reihenklemme.Value;
            }
        }

        private List<ReihenklemmeInfo> CreateReihenklemmen(TreeNodeDataOut nodeData, ref int currentKlemmeNrInLeiste)
        {
            if (nodeData.klemmleiste == "(intern)")
            {
                return new List<ReihenklemmeInfo>();
            }

            if (!TryGetLeitungstypConfiguration(nodeData, out var leitungstypCfg))
            {
                throw new NotSupportedException(
                    $"Konfiguration für Typ '{nodeData.Type}' und Leitungstyp '{nodeData.leitungstyp}' ist nicht vorhanden.");
            }

            var reihenklemmen = new List<ReihenklemmeInfo>();

            var nichtbearbeiteteAdern = nodeData.adern.ToList();

            while (nichtbearbeiteteAdern.Any())
            {
                if (TryGetKlemmeConfiguration(leitungstypCfg, nichtbearbeiteteAdern, out var klemmeCfg, out var adernFuerKlemme))
                {
                    // Reihenklemme erstellen
                    reihenklemmen.Add(new ReihenklemmeInfo(
                        klemmeCfg.ShortName, 
                        klemmeCfg.Description, 
                        CreateKlemmenListe(ref currentKlemmeNrInLeiste, adernFuerKlemme), 
                        klemmeCfg.Width,
                        adernFuerKlemme,
                        klemmeCfg.Color));


                    // nicht bearbeitete Adern aktualisieren
                    nichtbearbeiteteAdern.RemoveRange(adernFuerKlemme);
                }
                else
                {
                    throw new NotSupportedException(
                        $"Keine Klemmen-Konfiguration gefunden für '{string.Join(",", nichtbearbeiteteAdern.Select(x => x.klemmenTyp).ToArray())}'.");
                }
            }

            return reihenklemmen;
        }

        private bool TryGetKlemmeConfiguration(LeitungstypConfigurationElement leitungstypCfg, List<Adern> adern, out KlemmeConfigurationElement klemmeCfg, out List<Adern> usedAdern)
        {
            if (IsMehrstockklemmenModus(leitungstypCfg))
            {
                // größtmögliche Klemme, allerdings muss es für die restlichen Adern noch eine passende Klemme geben
                var klemmeCfgSortiert = leitungstypCfg.Klemmen.OfType<KlemmeConfigurationElement>()
                    .OrderBy(x => x.Typen.Length).ToList();

                for (int idx = klemmeCfgSortiert.Count-1; idx >=0; idx--)
                {
                    var adernFuerAktuelleSuche = adern.ToList();

                    var currentKlemmeCfg = klemmeCfgSortiert[idx];
                    if (currentKlemmeCfg.Typen.Length <= adern.Count || idx == 0)
                    {
                        // gibt es für die restlichen Adern noch eine passende Klemme?
                        usedAdern = adern.Take(currentKlemmeCfg.Typen.Length).ToList();

                        var anzVerbleibendeAdern = adernFuerAktuelleSuche.Count - usedAdern.Count;
                        if (anzVerbleibendeAdern != 0 && idx > 0 && anzVerbleibendeAdern < klemmeCfgSortiert[0].Typen.Length)
                        {
                            currentKlemmeCfg = klemmeCfgSortiert[idx-1];
                            usedAdern = adern.Take(currentKlemmeCfg.Typen.Length).ToList();
                        }

                        adernFuerAktuelleSuche.RemoveRange(usedAdern);
                        klemmeCfg = currentKlemmeCfg;
                        return true;
                    }
                }

            }

            foreach (KlemmeConfigurationElement currentKlemmeCfg in leitungstypCfg.Klemmen)
            {
                var alleAdern = adern.ToList();
                usedAdern = new List<Adern>();
                bool valid = true;

                // nur die ersten x Adern betrachten
                var adernFuerAktuelleSuche = alleAdern.Take(currentKlemmeCfg.Typen.Length).ToList();
                foreach (var typ in currentKlemmeCfg.Typen)
                {
                    bool optional = false;
                    var currentTyp = typ;
                    if (IsOptional(typ))
                    {
                        optional = true;
                        currentTyp = typ.Trim(new[] {'[', ']'});
                    }

                    var foundAder = adernFuerAktuelleSuche.FirstOrDefault(x => IsKlemmenTyp(x.klemmenTyp, currentTyp));
                    if (foundAder == null && optional == false)
                    {
                        // suche abbrechen
                        valid = false;
                        break;
                    }

                    if (foundAder != null)
                    {
                        adernFuerAktuelleSuche.Remove(foundAder);
                        usedAdern.Add(foundAder);
                    }
                }

                if (valid)
                {
                    klemmeCfg = currentKlemmeCfg;
                    return true;
                }

            }

            klemmeCfg = null;
            usedAdern = null;
            return false;
        }

        private bool IsKlemmenTyp(string klemmenTyp, string searchPattern)
        {
            return klemmenTyp.StartsWith(searchPattern) || (searchPattern == "out" && klemmenTyp == "reserve") || searchPattern == "*";
        }

        /// <summary>
        /// Wenn alle Klemmen "*" als typ haben
        /// </summary>
        /// <param name="leitungstypCfg"></param>
        /// <returns></returns>
        private bool IsMehrstockklemmenModus(LeitungstypConfigurationElement leitungstypCfg)
        {

            foreach (KlemmeConfigurationElement klemmeCfg in leitungstypCfg.Klemmen)
            {
                foreach (var typ in klemmeCfg.Typen)
                {
                    if (typ != "*")
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsOptional(string typ)
        {
            return typ.StartsWith("[") && typ.EndsWith("]");
        }

        private bool TryGetLeitungstypConfiguration(TreeNodeDataOut nodeData, out LeitungstypConfigurationElement configuration)
        {
            var leitungstyp = CreateLeitungstypWithQuerschnitt(nodeData);

            float.TryParse(nodeData.querschnitt, out var querschnitt);

            foreach (LeitungstypConfigurationElement leitungstypElement in _configLeitungstypConfigurations)
            {
                if (!string.IsNullOrEmpty(leitungstypElement.Typ) && nodeData.Type != leitungstypElement.Typ)
                {
                    continue;
                }

                if (!leitungstypElement.Leitungstypen.Any() ||
                    leitungstypElement.Leitungstypen.Any(x => leitungstyp.Contains(x)) &&
                    querschnitt >= leitungstypElement.MinQ && querschnitt<= leitungstypElement.MaxQ)
                {
                    configuration = leitungstypElement;
                    return true;
                }
            }

            configuration = null;
            return false;
        }

        private static string CreateLeitungstypWithQuerschnitt(TreeNodeDataOut nodeData)
        {
            return $"{nodeData.leitungstyp} {nodeData.adern.Length}x{nodeData.querschnitt}";
        }

        private string CreateKlemmenListe(ref int currentKlemmeNrInLeiste, IEnumerable<Adern> adern)
        {
            string result = string.Empty;

            foreach (var ader in adern)
            {
                if (ader.klemmenTyp != "N" && ader.klemmenTyp != "PE")
                {
                    if (!string.IsNullOrEmpty(result))
                    {
                        result += ",";
                    }
                    result += (++currentKlemmeNrInLeiste).ToString();
                }
            }

            return result;
        }


        private void WriteLeitungsliste(ExcelWorksheet ws, IEnumerable<TreeNodeDataOut> outNodes)
        {
            int rowNo = 1;
            int leitungNr = 1;

            ws.Cells[rowNo, 1].Value = "Nr";
            ws.Column(1).Width = 4;

            ws.Cells[rowNo, 2].Value = "Klemmleiste";
            ws.Column(2).Width = 5;

            ws.Cells[rowNo, 3].Value = "Klemme";
            ws.Column(3).Width = 5;

            ws.Cells[rowNo, 4].Value = "L-Nummer";
            ws.Column(4).Width = 13;

            ws.Cells[rowNo, 5].Value = "Leitungsname";
            ws.Column(5).Width = 38;

            ws.Cells[rowNo, 6].Value = "Leitungstyp";
            ws.Column(6).Width = 12;

            ws.Cells[rowNo, 7].Value = "Anz. Adern";
            ws.Column(7).Width = 3;

            ws.Cells[rowNo, 8].Value = "Querschnitt";
            ws.Column(8).Width = 4;

            ws.Cells[rowNo, 9].Value = "Leitungstyp";
            ws.Column(9).Width = 16;

            ws.Cells[rowNo, 10].Value = "Individuelle Leitungsbezeichnung";
            ws.Column(10).Width = 40;

            rowNo++;
            foreach (var nodeData in outNodes.OrderBy(x => x.KlemmleisteNummer).ThenBy(x => x.klemmenBlockNummer))
            {
                //To set values in the spreadsheet use the Cells indexer.
                ws.Cells[rowNo, 1].Value = leitungNr++;
                ws.Cells[rowNo, 2].Value = nodeData.klemmleiste;
                ws.Cells[rowNo, 3].Value = nodeData.klemmenBlockNummer;
                ws.Cells[rowNo, 4].Value = nodeData.leitungsnummer;
                ws.Cells[rowNo, 5].Value = nodeData.leitungsname;
                ws.Cells[rowNo, 6].Value = nodeData.leitungstyp;
                ws.Cells[rowNo, 7].Value = nodeData.adern.Length;
                ws.Cells[rowNo, 8].Value = nodeData.querschnitt;
                ws.Cells[rowNo, 9].Value = CreateLeitungstypWithQuerschnitt(nodeData);

                if (nodeData.individuelleLeitungsbezeichnungActive)
                {
                    ws.Cells[rowNo, 10].Value = $"{nodeData.leitungsnummer}\n{nodeData.individuelleLeitungsbezeichnung}";
                }

                ++rowNo;
            }
        }

    }
}