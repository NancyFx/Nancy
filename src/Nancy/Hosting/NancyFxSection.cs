namespace Nancy.Hosting
{
    using System;
    using System.Configuration;

    public class NancyFxSection : ConfigurationSection
    {
        [ConfigurationProperty("bootstrapper")]
        public BootStrapperElement BootStrapper
        {
            get
            {
                return (BootStrapperElement)this["bootstrapper"];
            }
            set
            {
                this["bootstrapper"] = value;
            }
        }

        public class BootStrapperElement : ConfigurationElement
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
