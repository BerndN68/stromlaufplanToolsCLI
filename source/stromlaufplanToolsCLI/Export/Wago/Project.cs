using System;
using System.Xml.Serialization;

namespace stromlaufplanToolsCLI.Export.Wago
{
    public class Project
    {
        [XmlAttribute]
        public string GUID = Guid.NewGuid().ToString();

        [XmlAttribute]
        public string Name;

        public Carrier Carrier { get; set; }
    }
}