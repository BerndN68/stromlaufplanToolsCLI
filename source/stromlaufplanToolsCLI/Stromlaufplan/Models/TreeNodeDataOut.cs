namespace stromlaufplanToolsCLI.Stromlaufplan.Models
{
    public class TreeNodeDataOut : TreeNodeDataBase
    {
        public string leitungsname;
        public string leitungsnummer;
        public string leitungstyp;
        public string querschnitt;
        public string klemmleiste;
        public string klemmenBeschriftungAutomatisch;
        public int klemmenBlockNummer;
        public bool individuelleLeitungsbezeichnungActive;
        public string individuelleLeitungsbezeichnung;

        public Adern[] adern;


        public int KlemmleisteNummer
        {
            get
            {
                if (!_KlemmleisteNummer.HasValue)
                {
                    _KlemmleisteNummer = int.MaxValue;
                    if (int.TryParse(klemmleiste.Substring(1), out var value))
                    {
                        _KlemmleisteNummer = value;
                    }
                }

                return _KlemmleisteNummer.Value;
            }
        }

        public override string ToString()
        {
            return leitungsname;
        }

        private int? _KlemmleisteNummer;
    }
}