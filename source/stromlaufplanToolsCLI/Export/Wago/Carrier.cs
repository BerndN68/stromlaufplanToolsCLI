using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace stromlaufplanToolsCLI.Export.Wago
{
    public class Carrier
    {
        [XmlAttribute]
        public string GUID = Guid.NewGuid().ToString();

        [XmlElement("Article_no")]
        public string ArticleNo;

        public string Name;

        [XmlArrayItem("Project_article"), XmlArray("Project_articles")]
        public List<Article> ProjectArticles = new List<Article>();
    }
}