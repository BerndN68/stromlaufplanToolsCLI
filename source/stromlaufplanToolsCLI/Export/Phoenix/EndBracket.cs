namespace stromlaufplanToolsCLI.Export.Phoenix
{
    /// <summary>
    ////Line number;
    ////Type(3);
    ////Article number;
    ////Not use attribute;
    ////Not use attribute;
    ////1 if rotated otherwise 0;
    ////Not use attribute;
    ////List of descriptions(separated by |);
    ////Comment
     /// </summary>
    public class EndBracket
    {
        public EndBracket()
        {
            Type = 3;
        }

        public long LineNumber { get; set; }

        public long Type { get; set; }

        public string ArticleNumber { get; set; }

        public string ArticleName { get; set; }

        public long Rotated { get; set; }

    }
}