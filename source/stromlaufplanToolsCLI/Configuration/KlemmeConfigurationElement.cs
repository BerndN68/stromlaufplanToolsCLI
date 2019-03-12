using System.Configuration;
using System.Drawing;

namespace stromlaufplanToolsCLI.Configuration
{
    public class KlemmeConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="KlemmeConfigurationElement"/>.
        /// </summary>
        public KlemmeConfigurationElement()
        {
            Properties.Add(new ConfigurationProperty(
                TYP_PROPERTY,
                typeof(string),
                null));

            Properties.Add(new ConfigurationProperty(
                SHORT_NAME_PROPERTY,
                typeof(string),
                null));

            Properties.Add(new ConfigurationProperty(
                COLOR_PROPERTY,
                typeof(Color),
                null));


            Properties.Add(new ConfigurationProperty(
                WIDTH_PROPERTY,
                typeof(int),
                null));

            Properties.Add(new ConfigurationProperty(
                DESCRIPTION_PROPERTY,
                typeof(string),
                null));

            Properties.Add(new ConfigurationProperty(
                ARTICLENO_PROPERTY,
                typeof(string),
                null));
        }


        public string[] Typen
        {
            get => ((string)this[TYP_PROPERTY]).Split('/');
            set
            {
                this[TYP_PROPERTY] = string.Join("/", value);
            }
        }

        public string TypString => (string)this[TYP_PROPERTY];

        
        /// <summary>
        /// Gets or sets the short name.
        /// </summary>
        public string ShortName
        {
            get => (string)this[SHORT_NAME_PROPERTY];
            set
            {
                this[SHORT_NAME_PROPERTY] = value;
            }
        }

        /// <summary>
        /// Gets or sets the value for description.
        /// </summary>
        public Color Color
        {
            get => (Color)this[COLOR_PROPERTY];
            set
            {
                this[COLOR_PROPERTY] = value;
            }
        }


        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width
        {
            get => (int)this[WIDTH_PROPERTY];
            set => this[WIDTH_PROPERTY] = value;
        }

        public string Description
        {
            get => (string)this[DESCRIPTION_PROPERTY];
            set
            {
                this[DESCRIPTION_PROPERTY] = value;
            }
        }

        public string ArticleNo
        {
            get => (string)this[ARTICLENO_PROPERTY];
            set
            {
                this[ARTICLENO_PROPERTY] = value;
            }
        }

        /// <summary>
        /// short name
        /// </summary>
        private const string SHORT_NAME_PROPERTY = "shortName";

        private const string TYP_PROPERTY = "typ";

        private const string COLOR_PROPERTY = "color";
        
        private const string WIDTH_PROPERTY = "width";

        private const string DESCRIPTION_PROPERTY = "description";

        private const string ARTICLENO_PROPERTY = "articleNo";
    }
}