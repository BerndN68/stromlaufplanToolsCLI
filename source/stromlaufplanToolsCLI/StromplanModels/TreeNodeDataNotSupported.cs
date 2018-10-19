namespace stromlaufplanToolsCLI.StromplanModels
{
    public class TreeNodeDataNotSupported : TreeNodeDataBase
    {
        private readonly string _jObjectString;

        public TreeNodeDataNotSupported(string jObjectString)
        {
            _jObjectString = jObjectString;
        }

        public override string ToString()
        {
            return _jObjectString;
        }
    }
}