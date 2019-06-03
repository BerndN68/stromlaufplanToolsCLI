namespace stromlaufplanToolsCLI.Export.Phoenix
{
    /// <summary>
    ////Line number;
    ////Type(5 or 6);
    ////Not use attribute;
    ////Not use attribute;
    ////Width[mm];
    ////Not use attribute;
    ////Not use attribute;
    ////Not use attribute;
    ////Comment
    /// </summary>
    public class FreeSpacing
    {
        public FreeSpacing()
        {
            Type = 5;
        }

        public long LineNumber { get; set; }

        public long Type { get; set; }

        public string Unused1 { get; set; }

        public string Unused2 { get; set; }

        public long Width { get; set; } = 20;

        public string Unused3 { get; set; }

        public string Unused4 { get; set; }

        public string Unused5 { get; set; }

        public string Comment { get; set; }

    }
}