using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using stromlaufplanToolsCLI.Stromlaufplan.Models;

namespace stromlaufplanToolsCLI.Commands
{
    public class ReihenklemmeInfo
    {
        public string ArticleNo { get; }

        public ReihenklemmeInfo(string name, string description, string articleNo, string klemmen, int width, IEnumerable<Adern> adern, Color color)
        {
            ArticleNo = articleNo;
            Klemmen = klemmen;
            Name = name;
            Description = description;
            Color = color;
            Adern = adern.ToList();
            Width = width;
        }

        public string Klemmen;

        public string Name;

        public string Description;

        public Color Color;

        public List<Adern> Adern;

        public int Width;
    }
}