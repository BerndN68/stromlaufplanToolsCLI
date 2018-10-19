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
                            (LeitungstypConfigurationSection)System.Configuration.ConfigurationManager.GetSection(
                                "Leitungstypen");

                        var command = new ExportKlemmlisteCommand(o.Token, o.Ids, o.OutputFilename, config.LeitungstypConfigurations);
                        command.Execute();
                    }

                });

        }

    }
}
