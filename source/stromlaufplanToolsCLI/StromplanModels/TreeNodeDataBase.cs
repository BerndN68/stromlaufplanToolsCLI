using Newtonsoft.Json;

namespace stromlaufplanToolsCLI.StromplanModels
{
    public abstract class TreeNodeDataBase
    {
        [JsonProperty("type")]
        public string Type;
    }
}