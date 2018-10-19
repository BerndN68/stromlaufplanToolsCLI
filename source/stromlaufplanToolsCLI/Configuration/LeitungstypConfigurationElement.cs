using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace stromlaufplanToolsCLI.Configuration
{
    public class LeitungstypConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="LeitungstypConfigurationElement"/>.
        /// </summary>
        public LeitungstypConfigurationElement()
        {
            Properties.Add(new ConfigurationProperty(
                KLEMMEN_PROPERTY,
                typeof(KlemmeElementCollection),
                null));

            Properties.Add(new ConfigurationProperty(
                TYP_PROPERTY,
                typeof(string),
                null));

            Properties.Add(new ConfigurationProperty(
                LEITUNGSTYP_PROPERTY,
                typeof(string),
                null));

            Properties.Add(new ConfigurationProperty(
                MIN_Q_PROPERTY,
                typeof(float),
                null));

            Properties.Add(new ConfigurationProperty(
                MAX_Q_PROPERTY,
                typeof(float),
                null));

            Properties.Add(new ConfigurationProperty(
                IGNORE_PROPERTY,
                typeof(bool),
                null));
        }


        private const string TYP_PROPERTY = "typ";

        private const string LEITUNGSTYP_PROPERTY = "leitungstyp";

        /// <summary>
        /// Kleinster Querschnitt
        /// </summary>
        private const string MIN_Q_PROPERTY = "minQ";

        /// <summary>
        /// größter Querschnitt
        /// </summary>
        private const string MAX_Q_PROPERTY = "maxQ";


        private const string KLEMMEN_PROPERTY = "Klemmen";

        private const string IGNORE_PROPERTY = "ignore";

        public string Typ
        {
            get => (string)this[TYP_PROPERTY];
            set => this[TYP_PROPERTY] = value;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string[] Leitungstypen
        {
            get => ((string)this[LEITUNGSTYP_PROPERTY]).Split(',');
            set => this[LEITUNGSTYP_PROPERTY] = string.Join(",", value);
        }

        public string LeitungstypString => (string)this[LEITUNGSTYP_PROPERTY];


        /// <summary>
        /// Gets or sets the min q.
        /// </summary>
        public float MinQ
        {
            get => (float)this[MIN_Q_PROPERTY];
            set
            {
                this[MIN_Q_PROPERTY] = value;
            }
        }

        /// <summary>
        /// Gets or sets the max q.
        /// </summary>
        public float MaxQ
        {
            get => (float)this[MAX_Q_PROPERTY];
            set
            {
                this[MAX_Q_PROPERTY] = value;
            }
        }

        public bool Ignore
        {
            get => (bool)this[IGNORE_PROPERTY];
            set
            {
                this[IGNORE_PROPERTY] = value;
            }
        }

        /// <summary>
        /// Gets or sets the commandline arguments.
        /// </summary>
        public KlemmeElementCollection Klemmen
        {
            get => (KlemmeElementCollection)this[KLEMMEN_PROPERTY];
            set
            {
                this[KLEMMEN_PROPERTY] = value;
            }
        }

    }
}
