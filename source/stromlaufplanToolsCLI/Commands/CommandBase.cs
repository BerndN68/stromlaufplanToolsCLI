using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using stromlaufplanToolsCLI.Json;

namespace stromlaufplanToolsCLI.Commands
{
    public abstract class CommandBase
    {
        private readonly string _token;

        public CommandBase(string token)
        {
            _token = token;
        }

        protected T ExecuteUrl<T>(string url)
        {
            var client = new RestClient($"https://beta.stromlaufplan.de/webapi/{url}");
            client.Authenticator = new JwtAuthenticator(_token);
            var request = new RestRequest(Method.GET);
            request.RequestFormat = DataFormat.Json;

            IRestResponse response = client.Execute(request);

            var data = JsonConvert.DeserializeObject<T>(response.Content, new JsonItemConverter());

            return data;
        }

        public abstract void Execute();
    }
}