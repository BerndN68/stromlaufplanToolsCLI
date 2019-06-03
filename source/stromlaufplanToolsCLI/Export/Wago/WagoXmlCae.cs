using System.Xml.Serialization;

namespace stromlaufplanToolsCLI.Export.Wago
{
    [XmlRoot("WAGO_XML_CAE")]
    public class WagoXmlCae
    {
        public Project Project { get; set; }
    }
}