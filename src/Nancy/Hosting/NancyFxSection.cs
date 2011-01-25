namespace Nancy.Hosting
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
            public String Type
            {
                get
                {
                    return (String)this["type"];
                }
                set
                {
                    this["type"] = value;
                }
            }

            [ConfigurationProperty("assembly", DefaultValue = "", IsRequired = true)]
            public String Assembly
            {
                get
                {
                    return (String)this["assembly"];
                }
                set
                {
                    this["assembly"] = value;
                }
            }
        }
    }
}
