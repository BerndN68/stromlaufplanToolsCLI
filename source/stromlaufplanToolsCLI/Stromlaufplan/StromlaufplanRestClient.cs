using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using stromlaufplanToolsCLI.Json;
using stromlaufplanToolsCLI.Stromlaufplan.Models;

namespace stromlaufplanToolsCLI.Stromlaufplan
{
    public class StromlaufplanRestClient
    {
        private readonly string _apiToken;

        public StromlaufplanRestClient(string apiToken)
        {
            _apiToken = apiToken;
        }

        public Project[] GetProjects()
        {
            return ExecuteUrl<Project[]>("projects");
        }

        public PlanData GetData(string projectId)
        {
            return ExecuteUrl<PlanData>($"projects/{projectId}/export/json");
        }

        private T ExecuteUrl<T>(string url)
        {
            var client = new RestClient($"https://beta.stromlaufplan.de/webapi/{url}");
            client.Authenticator = new JwtAuthenticator(_apiToken);
            var request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);

            var data = JsonConvert.DeserializeObject<T>(response.Content, new JsonItemConverter());

            return data;
        }
    }
}