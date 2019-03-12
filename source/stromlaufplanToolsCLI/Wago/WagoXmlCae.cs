using System.Xml;
using System.Xml.Serialization;

namespace stromlaufplanToolsCLI.Wago
{
    [XmlRoot("WAGO_XML_CAE")]
    public class WagoXmlCae
    {
        public Project Project { get; set; }
    }
}