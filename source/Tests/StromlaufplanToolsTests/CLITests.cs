using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using stromlaufplanToolsCLI;

namespace StromlaufplanToolsTests
{
    /// <summary>
    /// Ruft die statische Main Methode mit verschiedenen Parametern auf und prüft ob die erwarteten Dateien
    /// im Ausgabeverzeichnis vorhanden sind.
    /// </summary>
    [TestClass]
    public class CLITests
    {
        public TestContext TestContext { get; set; }

        private string mApiToken;

        [TestInitialize]
        public void TestInitialize()
        {
            mApiToken =
                TestContext.Properties["ApiToken"].ToString();
        }




    }
}
