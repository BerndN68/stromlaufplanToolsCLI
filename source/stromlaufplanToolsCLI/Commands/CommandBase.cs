using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using stromlaufplanToolsCLI.Json;
using stromlaufplanToolsCLI.Stromlaufplan;

namespace stromlaufplanToolsCLI.Commands
{
    public abstract class CommandBase
    {
        private readonly string _token;

        public CommandBase(string token)
        {
            RestClient = new StromlaufplanRestClient(token);
        }

        public StromlaufplanRestClient RestClient { get; }

        public abstract void Execute();
    }
}