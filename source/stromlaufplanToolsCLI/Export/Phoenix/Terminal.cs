namespace stromlaufplanToolsCLI.Export.Phoenix
{
    /// <summary>
    /// Line number;
    ////Type(1);
    ////Article number;
    ////Not use attribute(Name);
    ////Not use attribute;
    ////1 if rotated otherwise 0;
    ////Coordinates for switch terminal;
    ////List of descriptions(separated by |);
    ////Comment;
    ////Not use attribute;
    ////Accessories article number list(separated by |);
    /// </summary>
    public class Terminal
    {
        public Terminal()
        {
            Type = 1;
        }

        public long LineNumber { get; set; }

        public long Type { get; set; }

        public string ArticleNumber { get; set; }

        public string ArticleName { get; set; }

        public long Rotated { get; set; }

        public string CoordinatsUnused { get; set; }

        public string DescriptionsUnused { get; set; }

        public string CommentUnused { get; set; }

        public string Unused { get; set; }

        public string Accessories { get; set; }

    }
}