using System;
using stromlaufplanToolsCLI.StromplanModels;

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
            var result = ExecuteUrl<Project[]>("projects");

            Console.WriteLine("Liste der verfügbaren Projekte:");
            foreach (var project in result)
            {
                Console.WriteLine($"{project.name}: id = {project.id}");
            }

            Console.WriteLine("ok");
            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
        }
    }
}