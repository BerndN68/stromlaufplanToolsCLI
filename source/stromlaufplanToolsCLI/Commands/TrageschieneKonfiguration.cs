namespace stromlaufplanToolsCLI.Commands
{
    public class TrageschieneKonfiguration
    {
        public TrageschieneKonfiguration(string name, string[] klemmleisten)
        {
            Name = name;
            Klemmleisten = klemmleisten;
        }

        public string Name { get; }
        public string[] Klemmleisten { get; }
    }
}