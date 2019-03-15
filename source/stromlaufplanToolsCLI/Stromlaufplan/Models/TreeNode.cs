using System.Collections.Generic;
using Newtonsoft.Json;

namespace stromlaufplanToolsCLI.Stromlaufplan.Models
{
    public class TreeNode
    {
        public string id;

        public string text;

        [JsonProperty("type")]
        public string Type;

        public List<TreeNode> children;
    }
}