using Newtonsoft.Json;

namespace stromlaufplanToolsCLI.Stromlaufplan.Models
{
    public abstract class TreeNodeDataBase
    {
        [JsonProperty("type")]
        public string Type;
    }
}