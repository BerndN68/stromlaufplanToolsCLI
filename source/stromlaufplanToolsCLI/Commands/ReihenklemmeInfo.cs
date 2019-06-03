using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using stromlaufplanToolsCLI.Stromlaufplan.Models;

namespace stromlaufplanToolsCLI.Commands
{
    public class ReihenklemmeInfo
    {
        public string ArticleNo { get; }

        public string ArticleName { get; }

        public ReihenklemmeInfo() { }

        public ReihenklemmeInfo(string name, string producer, string articleNo, string articleName, string klemmen, int width, IEnumerable<Adern> adern, Color color)
        {
            ArticleNo = articleNo;
            ArticleName = articleName;
            Klemmen = klemmen;
            Name = name;
            Producer = producer;
            Color = color;
            Adern = adern.ToList();
            Width = width;
        }

        public ReihenklemmeInfo(string articleNo, string articleName)
        {
            ArticleNo = articleNo;
            ArticleName = articleName;
        }

        public string Klemmen;

        public string Name;

        public string Producer;

        public Color Color;

        public List<Adern> Adern;

        public int Width;
    }
}