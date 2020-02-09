using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using stromlaufplanToolsCLI.Common;
using stromlaufplanToolsCLI.Stromlaufplan.Models;

namespace stromlaufplanToolsCLI.Commands
{
    public class ExportKomponentenCommand : CommandBase
    {
        private readonly IEnumerable<string> _ids;
        private string _outputPath;

        public ExportKomponentenCommand(
            string token,
            IEnumerable<string> ids,
            string outputPath)
            : base(token)
        {
            _ids = ids;
            _outputPath = outputPath;
        }

        public override void Execute()
        {
            Console.WriteLine("Komponenten exportieren");

            // Ausgabeverzeichnis erstellen
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }

            var outputFileName = Path.Combine(_outputPath, "export.xlsx");

            //Creates a blank workbook. Use the using statment, so the package is disposed when we are done.
            using (var xlPackage = new ExcelPackage())
            {
                foreach (var projectId in _ids)
                {

                    Console.Write($"Projektdaten für Projekt {projectId} von stromlaufplan.de lesen ... ");
                    var data = RestClient.GetData(projectId);
                    Console.WriteLine("ok");


                    ExportKomponenten(xlPackage, data);
                }

                //Save the new workbook. We haven't specified the filename so use the Save as method.
                Console.Write($"Excel Datei speichern {outputFileName} ... ");

                xlPackage.SaveAs(new FileInfo(outputFileName));
                Console.WriteLine("ok");
            }

        }


        private void ExportKomponenten(ExcelPackage xlPackage, PlanData data)
        {
            const int PROJECT_NAME_MAX_LENGTH = 15;
            var projectName = data.documentData.projectName.Truncate(PROJECT_NAME_MAX_LENGTH);

            //A workbook must have at least on cell, so lets add one... 
            Console.Write($"Datenliste erstellen ... ");
            var ws = xlPackage.Workbook.Worksheets.Add($"{projectName}-Komponenten");
            WriteKomponentenliste(ws, data);
            Console.WriteLine("ok");
        }

        private void WriteKomponentenliste(ExcelWorksheet ws, PlanData data)
        {
            var treeNodes = data.treeNodes
                .First(x => x.text == "KNX-Komponenten").children.ToList();

            int rowNo = 1;

            ws.Cells[rowNo, 1].Value = "Text";
            ws.Column(1).Width = 40;

            ws.Cells[rowNo, 2].Value = "Typ";
            ws.Column(2).Width = 40;

            WriteTreeNodes(ws, treeNodes, ref rowNo);
        }

        private void WriteTreeNodes(ExcelWorksheet ws, IEnumerable<TreeNode> treeNodes, ref int rowNo)
        {
            foreach (var nodeData in treeNodes)
            {
                ++rowNo;
                ws.Cells[rowNo, 1].Value = nodeData.text;
                ws.Cells[rowNo, 2].Value = nodeData.Type;

                WriteTreeNodes(ws, nodeData.children, ref rowNo);
            }
        }

    }
}