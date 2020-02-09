using System.Configuration;

namespace stromlaufplanToolsCLI.Configuration
{
    public class LeitungstypConfigurationElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new element.
        /// </summary>
        /// <returns>A new element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new LeitungstypConfigurationElement();
        }

        /// <summary>
        /// Gets the element key.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The key.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var leitungstyp = ((LeitungstypConfigurationElement) element);
            return $"{leitungstyp.LeitungstypString}-{leitungstyp.MinQ}-{leitungstyp.MaxQ}";
        }

        /// <summary>
        /// Adds a setting to the processes.
        /// </summary>
        /// <param name="configurationElement">The configuration element.</param>
        public void Add(LeitungstypConfigurationElement configurationElement)
        {
            BaseAdd(configurationElement);
        }

        /// <summary>
        /// Removes the configuration for the specified process.
        /// </summary>
        /// <param name="process">The process to remove.</param>
        public void Remove(string process)
        {
            BaseRemove(process);
        }


    }
}