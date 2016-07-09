namespace Nancy.Hosting.Aspnet
{
    using System;
    using System.Configuration;

    public class NancyFxSection : ConfigurationSection
    {
        [ConfigurationProperty("disableoutputbuffer")]
        public DisableOutputBufferElement DisableOutputBuffer
        {
            get { return (DisableOutputBufferElement)this["disableoutputbuffer"]; }
            set { this["disableoutputbuffer"] = value; }
        }

        [ConfigurationProperty("bootstrapper")]
        public BootstrapperElement Bootstrapper
        {
            get { return (BootstrapperElement)this["bootstrapper"]; }
            set { this["bootstrapper"] = value; }
        }

        public class DisableOutputBufferElement : ConfigurationElement
        {
            [ConfigurationProperty("value", DefaultValue = false, IsRequired = true)]
            public bool Value
            {
                get { return (bool)this["value"]; }
                set { this["value"] = value; }
            }
        }

        public class BootstrapperElement : ConfigurationElement
        {
            [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
            public string Type
            {
                get { return (string)this["type"]; }
                set { this["type"] = value; }
            }

            [ConfigurationProperty("assembly", DefaultValue = "", IsRequired = true)]
            public string Assembly
            {
                get { return (String)this["assembly"]; }
                set { this["assembly"] = value; }
            }
        }
    }
}
