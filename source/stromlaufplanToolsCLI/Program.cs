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
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {

                    if (o.List)
                    {
                        var command = new ProjectListCommand(o.Token);
                        command.Execute();
                    }

                    if (o.Klemmenplan)
                    {
                        var config =
                            (LeitungstypConfigurationSection)ConfigurationManager.GetSection(
                                "Leitungstypen");
                        var reihenklemmenCfg = (NameValueCollection)ConfigurationManager.GetSection("Reihenklemmen");


                        var command = new ExportKlemmlisteCommand(o.Token, o.Ids, o.OutputFilename, o.WagoXML, o.TragschienenKonfiguration, config.LeitungstypConfigurations, reihenklemmenCfg);
                        command.Execute();
                    }


                });

        }

    }
}
