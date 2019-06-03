namespace stromlaufplanToolsCLI.Export.Phoenix
{
    /// <summary>
    /// Line number;
    ////Type(100);
    ////Article number;
    ////Name;
    ////User name;
    ////Length[mm];
    ////Edge of cut[mm];
    ////List X-size[mm] drill holes(separated by |);
    ////List types of drill holes(0 if x = y, 1 if x > y, 2 if x<y, 3 for nut and 4 for screw, separated by |);
    ////List Y-size[mm] drill holes(separated by |);
    ////List distance[mm] from left edge of the rail for every drill holes(separated by |);
    ////Not use attribute;
    ////Drill type(0 for user, 1 for standard);
    ////End cap article number if have to be add;
    ////End cap name;
    ////End cap width[mm];
    ////End cap depth[mm];
    ////Rail’s accessories article numbers list(separated by |)
    /// </summary>
    public class Rail
    {
        public Rail()
        {
            Type = 100;
        }

        public long LineNumber { get; set; }

        public long Type { get; set; }

        public string ArticleNumber { get; set; }

        public string ArticleName { get; set; }

        public string Name { get; set; }

        public long LengthInMM { get; set; } = 2000;

        public long EdgeOfCut { get; set; } = 20;

        public long ListXSize { get; set; } = 10;

        public long ListType { get; set; } = 4;

        public long ListYSize { get; set; } = 2;

        public long ListDistance { get; set; } = 5;

    }
}