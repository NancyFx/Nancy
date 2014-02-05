namespace Nancy.Hosting.Aspnet
{
    using System;
    using System.Configuration;

    public class NancyFxSection : ConfigurationSection
    {
        [ConfigurationProperty("bootstrapper")]
        public BootstrapperElement Bootstrapper
        {
            get
            {
                return (BootstrapperElement)this["bootstrapper"];
            }
            set
            {
                this["bootstrapper"] = value;
            }
        }

        public class BootstrapperElement : ConfigurationElement
        {
            [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
            public string Type
            {
                get
                {
                    return (string)this["type"];
                }
                set
                {
                    this["type"] = value;
                }
            }

            [ConfigurationProperty("assembly", DefaultValue = "", IsRequired = true)]
            public string Assembly
            {
                get
                {
                    return (string)this["assembly"];
                }
                set
                {
                    this["assembly"] = value;
                }
            }
        }
    }
}
