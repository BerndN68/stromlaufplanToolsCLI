using System;
using System.Xml.Serialization;

namespace stromlaufplanToolsCLI.Wago
{
    public class Article
    {
        [XmlAttribute]
        public string GUID = Guid.NewGuid().ToString();

        [XmlElement("Article_no")]
        public string ArticleNo;

        [XmlElement("Project_article_marking")]
        public Marking Marking;

        [XmlElement("Position")]
        public int Position;
    }
}