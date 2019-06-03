using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using stromlaufplanToolsCLI.Configuration;

namespace stromlaufplanToolsCLI.Commands
{
    public abstract class ExportDataCommandBase : CommandBase
    {
        protected readonly IEnumerable<string> _ids;
        protected readonly string _outputPath;
        protected readonly string _tragschienenKonfiguration;
        protected readonly NameValueCollection _reihenklemmenCfg;
        protected readonly ReihenklemmenCreator _reihenklemmenCreator;

        public ExportDataCommandBase(
            string token,
            IEnumerable<string> ids,
            string outputPath,
            string tragschienenKonfiguration,
            LeitungstypConfigurationElementCollection configLeitungstypConfigurations,
            NameValueCollection reihenklemmenCfg)
            : base(token)
        {
            _ids = ids;
            _outputPath = outputPath;
            _tragschienenKonfiguration = tragschienenKonfiguration;
            _reihenklemmenCreator = new ReihenklemmenCreator(configLeitungstypConfigurations, "Phoenix");
            _reihenklemmenCfg = reihenklemmenCfg;
        }

        protected void EnsureOutputPathExists()
        {
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

        protected List<TrageschieneKonfiguration> CreateEineTrageschieneKonfigurationProBlock(string projectId, List<string> alleKlemmleisten)
        {
            var tragschienenKonfiguration = new List<TrageschieneKonfiguration>();

            foreach (var klemmleiste in alleKlemmleisten)
            {
                tragschienenKonfiguration.Add(new TrageschieneKonfiguration(klemmleiste, new string[] { klemmleiste }));
            }

            return tragschienenKonfiguration;
        }

        protected List<TrageschieneKonfiguration> CreateTrageschienenKonfigurationFromKonfiguration(
            string projectId,
            List<string> alleKlemmleisten)
        {
            var tragschienenKonfiguration = new List<TrageschieneKonfiguration>();

            var konfigurationenAlleProjekte = _tragschienenKonfiguration?.Split('|') ?? new string[] { };
            foreach (var konfigurationProjekt in konfigurationenAlleProjekte)
            {
                var array = konfigurationProjekt.Split(':');
                if (array[0] == projectId)
                {
                    // gefundene Konfiguration weiter zerlegen
                    var tragschienenArray = array[1].Split(';');
                    foreach (var tragschiene in tragschienenArray)
                    {
                        var klemmleisten = tragschiene.Split(',');
                        tragschienenKonfiguration.Add(new TrageschieneKonfiguration(klemmleisten.First(), klemmleisten));
                    }
                }
            }

            // alle noch fehlenden Klemmeleisten zu einer neuen Tragschiene hinzufügen
            var fehlendeKlemmleisten =
                alleKlemmleisten.Except(tragschienenKonfiguration.SelectMany(x => x.Klemmleisten)).ToList();
            if (fehlendeKlemmleisten.Any())
            {
                tragschienenKonfiguration.Add(new TrageschieneKonfiguration(fehlendeKlemmleisten.First(), fehlendeKlemmleisten.ToArray()));
            }

            return tragschienenKonfiguration;
        }
    }
}