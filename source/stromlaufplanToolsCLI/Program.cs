using System.Collections.Specialized;
using System.Configuration;
using CommandLine;
using stromlaufplanToolsCLI.Commands;
using stromlaufplanToolsCLI.Configuration;

namespace stromlaufplanToolsCLI
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(RunWithOptions);
        }

        internal static void RunWithOptions(Options options)
        {
            if (options.List)
            {
                var command = new ProjectListCommand(options.Token);
                command.Execute();
            }

            if (options.Klemmenplan)
            {
                var config =
                    (LeitungstypConfigurationSection)ConfigurationManager.GetSection(
                        "Leitungstypen");


                var command = new ExportKlemmlisteCommand(
                    options.Token,
                    options.Ids,
                    options.OutputFilename,
                    config.LeitungstypConfigurations);
                command.Execute();
            }

            if (options.WagoXML)
            {
                var config =
                    (LeitungstypConfigurationSection)ConfigurationManager.GetSection(
                        "Leitungstypen");
                var reihenklemmenCfg = (NameValueCollection)ConfigurationManager.GetSection("Reihenklemmen");


                var command = new ExportWagoXmlCommand(
                    options.Token,
                    options.Ids,
                    options.OutputFilename,
                    options.TragschienenKonfiguration,
                    config.LeitungstypConfigurations,
                    reihenklemmenCfg);
                command.Execute();
            }
        }

    }
}
