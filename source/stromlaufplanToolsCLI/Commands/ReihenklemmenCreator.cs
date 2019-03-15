using System;
using System.Collections.Generic;
using System.Linq;
using stromlaufplanToolsCLI.Common;
using stromlaufplanToolsCLI.Configuration;
using stromlaufplanToolsCLI.Stromlaufplan.Models;

namespace stromlaufplanToolsCLI.Commands
{
    public class ReihenklemmenCreator
    {
        private readonly LeitungstypConfigurationElementCollection _configLeitungstypConfigurations;

        public ReihenklemmenCreator(
            LeitungstypConfigurationElementCollection configLeitungstypConfigurations)
        {
            _configLeitungstypConfigurations = configLeitungstypConfigurations;
        }

        public List<ReihenklemmeInfo> CreateReihenklemmen(TreeNodeDataOut nodeData, ref int currentKlemmeNrInLeiste)
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
                        klemmeCfg.ArticleNo,
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

                for (int idx = klemmeCfgSortiert.Count - 1; idx >= 0; idx--)
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
                            currentKlemmeCfg = klemmeCfgSortiert[idx - 1];
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
                        currentTyp = typ.Trim(new[] { '[', ']' });
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
                    querschnitt >= leitungstypElement.MinQ && querschnitt <= leitungstypElement.MaxQ)
                {
                    configuration = leitungstypElement;
                    return true;
                }
            }

            configuration = null;
            return false;
        }

        public static string CreateLeitungstypWithQuerschnitt(TreeNodeDataOut nodeData)
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

    }
}