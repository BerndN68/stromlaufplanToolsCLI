using System;
using stromlaufplanToolsCLI.Stromlaufplan.Models;

namespace stromlaufplanToolsCLI.Commands
{
    public class ProjectListCommand : CommandBase
    {

        public ProjectListCommand(string token)
            : base(token)
        {
        }

        public override void Execute()
        {
            var projects = RestClient.GetProjects();

            Console.WriteLine("Liste der verfügbaren Projekte:");
            foreach (var project in projects)
            {
                Console.WriteLine($"{project.name}: id = {project.id}");
            }

            Console.WriteLine("ok");
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
        }
    }
}