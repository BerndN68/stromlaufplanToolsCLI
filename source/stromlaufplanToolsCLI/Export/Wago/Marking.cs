using System.Collections.Generic;
using System.Xml.Serialization;

namespace stromlaufplanToolsCLI.Export.Wago
{
    public class Marking
    {
        public int Size = 11;

        [XmlArrayItem("Line"), XmlArray("Lines")]
        public List<string> Lines = new List<string>();
    }
}