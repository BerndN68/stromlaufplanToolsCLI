namespace stromlaufplanToolsCLI.Export.Phoenix.Extras
{
    /// <summary>
    ////Name Tag;
    ////Not use attribute;
    ////Article number;
    ////Not use attribute(Name);
    ////Index terminal where accessory is inserted;
    ////Coordinates;
    ////List of descriptions(separated by |);
    ////Additional accessory article number
    /// </summary>
    public class Accessories
    {
        public long NameTag { get; set; } = 99998;

        public string Unused { get; set; }

        public string ArticleNumber { get; set; }

        public string ArticleName { get; set; }

        public long Index { get; set; } = 1;

        public long Coordinates { get; set; } = 1;

        public string Descriptions { get; set; }

        public string AdditionalAccessory { get; set; }

    }
}