using System.Configuration;

namespace stromlaufplanToolsCLI.Configuration
{
    public class LeitungstypConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// Gets processes.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        public LeitungstypConfigurationElementCollection LeitungstypConfigurations =>
            (LeitungstypConfigurationElementCollection) this[string.Empty];
    }
}
