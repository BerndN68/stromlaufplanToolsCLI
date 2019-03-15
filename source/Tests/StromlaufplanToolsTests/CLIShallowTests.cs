using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using stromlaufplanToolsCLI;
using stromlaufplanToolsCLI.Stromlaufplan;

namespace StromlaufplanToolsTests
{
    /// <summary>
    /// Ruft die statische Main Methode mit verschiedenen Parametern auf und prüft ob die erwarteten Dateien
    /// im Ausgabeverzeichnis vorhanden sind.
    /// </summary>
    [TestClass]
    public class CLIShallowTests
    {
        public TestContext TestContext { get; set; }

        private string[] mApiTokensShallowTests = { };

        [TestInitialize]
        public void TestInitialize()
        {
            mApiTokensShallowTests =
                TestContext.Properties["ApiTokensShallowTests"].ToString()
                    .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
        }

        [TestMethod]
        public void Read_List_of_available_projects()
        {
            // Arrange
            Action<Options> prepareOptions = o => o.List = true;

            // Act
            RunWithApiTokens(prepareOptions, o => {} );

            // Assert
        }

        [TestMethod]
        public void Export_Klemmliste_using_OutputPathWithFilename()
        {
            // Arrange
            Action<Options> prepareOptions = o =>
            {
                var restClient = new StromlaufplanRestClient(o.Token);
                var projects = restClient.GetProjects();

                o.Klemmenplan = true;
                o.OutputFilename = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName, o.Token, "Klemmenplan.xlsx");
                o.Ids = projects.Select(x => x.id);
            };

            Action<Options> verifyResult = o =>
            {
                File.Exists(o.OutputFilename).Should().BeTrue();
            };

            // Act
            RunWithApiTokens(prepareOptions, verifyResult);

            // Assert
        }

        [TestMethod]
        public void Export_Klemmliste_using_OutputPathWithoutFilename()
        {
            // Arrange
            Action<Options> prepareOptions = o =>
            {
                var restClient = new StromlaufplanRestClient(o.Token);
                var projects = restClient.GetProjects();

                o.Klemmenplan = true;
                o.OutputFilename = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName, o.Token) +"/";
                o.Ids = projects.Select(x => x.id);
            };

            Action<Options> verifyResult = o =>
            {
                File.Exists(Path.Combine(o.OutputFilename, "export.xlsx")).Should().BeTrue();
            };

            // Act
            RunWithApiTokens(prepareOptions, verifyResult);

            // Assert
        }

        [TestMethod]
        public void Export_WagoXml_using_OutputPathWithFilename()
        {
            // Arrange
            Action<Options> prepareOptions = o =>
            {
                var restClient = new StromlaufplanRestClient(o.Token);
                var projects = restClient.GetProjects();

                o.WagoXML = true;
                o.OutputFilename = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName, o.Token, "sample.xlsx");
                o.Ids = projects.Select(x => x.id);
            };

            Action<Options> verifyResult = o =>
            {
                var resultFiles = Directory.EnumerateFiles(Path.GetDirectoryName(o.OutputFilename)).ToList();
                resultFiles.Count.Should().BeGreaterThan(0);
                resultFiles.ForEach(x => Path.GetExtension(x).Should().Be(".xml"));
            };

            // Act
            RunWithApiTokens(prepareOptions, verifyResult);

            // Assert
        }

        [TestMethod]
        public void Export_WagoXml_using_OutputPathWithoutFilename()
        {
            // Arrange
            Action<Options> prepareOptions = o =>
            {
                var restClient = new StromlaufplanRestClient(o.Token);
                var projects = restClient.GetProjects();

                o.WagoXML = true;
                o.OutputFilename = Path.Combine(TestContext.TestRunResultsDirectory, TestContext.TestName, o.Token) + "/";
                o.Ids = projects.Select(x => x.id);
            };

            Action<Options> verifyResult = o =>
            {
                var resultFiles = Directory.EnumerateFiles(o.OutputFilename).ToList();
                resultFiles.Count.Should().BeGreaterThan(0);
                resultFiles.ForEach( x => Path.GetExtension(x).Should().Be(".xml"));
            };

            // Act
            RunWithApiTokens(prepareOptions, verifyResult);

            // Assert
        }

        private void RunWithApiTokens( Action<Options> prepareOptions, Action<Options> verifyResult)
        {
            foreach (var apiToken in mApiTokensShallowTests)
            {
                var options = new Options{ Token = apiToken };
                prepareOptions(options);

                Program.RunWithOptions(options);

                verifyResult(options);
            }
        }


    }
}
