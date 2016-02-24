namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;

    public class RazorConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("disableAutoIncludeModelNamespace", DefaultValue = "false", IsRequired = false)]
        public Boolean DisableAutoIncludeModelNamespace
        {
            get { return (Boolean)this["disableAutoIncludeModelNamespace"]; }
            set { this["disableAutoIncludeModelNamespace"] = value; }
        }

        [ConfigurationProperty("assemblies", IsRequired = false)]
        public AssemblyConfigurationCollection Assemblies
        {
            get { return this["assemblies"] as AssemblyConfigurationCollection; }
            set { this["assemblies"] = value; }
        }

        [ConfigurationProperty("namespaces", IsRequired = false)]
        public NamespaceConfigurationCollection Namespaces
        {
            get { return this["namespaces"] as NamespaceConfigurationCollection; }
            set { this["namespaces"] = value; }
        }
    }

    public sealed class AssemblyConfigurationItem : ConfigurationElement
    {
        // repeat this pattern for each additional attribute you want in the <add /> tag.
        // Only the assembly="foo.dll" portion is defined in this class, and is accessed
        // via the AssemblyName property.
        public const string AssemblyPropertyName = "assembly";

        [ConfigurationProperty(AssemblyPropertyName, IsRequired = true, IsKey = true)]
        public string AssemblyName
        {
            get { return this[AssemblyPropertyName] as string; }
            set { this[AssemblyPropertyName] = value; }
        }
    }

    public class AssemblyConfigurationCollection : ConfigurationElementCollection, IEnumerable<AssemblyConfigurationItem>
    {
        public const string PluginsElementName = "assembly";

        protected override string ElementName
        {
            get { return PluginsElementName; }
        }

        // this is extraneous, but I find it very useful for enumerating over a configuration collection in a type-safe manner.

        #region IEnumerable<AssemblyConfigurationItem> Members

        public new IEnumerator<AssemblyConfigurationItem> GetEnumerator()
        {
            foreach (AssemblyConfigurationItem item in (this as IEnumerable))
            {
                yield return item;
            }
        }

        #endregion

        protected override ConfigurationElement CreateNewElement()
        {
            return new AssemblyConfigurationItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssemblyConfigurationItem)element).AssemblyName;
        }
    }

    public sealed class NamespaceConfigurationItem : ConfigurationElement
    {
        // repeat this pattern for each additional attribute you want in the <add /> tag.
        // Only the assembly="foo.dll" portion is defined in this class, and is accessed
        // via the AssemblyName property.
        public const string NamespacePropertyName = "namespace";

        [ConfigurationProperty(NamespacePropertyName, IsRequired = true, IsKey = true)]
        public string NamespaceName
        {
            get { return this[NamespacePropertyName] as string; }
            set { this[NamespacePropertyName] = value; }
        }
    }

    public class NamespaceConfigurationCollection : ConfigurationElementCollection, IEnumerable<NamespaceConfigurationItem>
    {
        public const string PluginsElementName = "namespace";

        protected override string ElementName
        {
            get { return PluginsElementName; }
        }

        // this is extraneous, but I find it very useful for enumerating over a configuration collection in a type-safe manner.

        #region IEnumerable<NamespaceConfigurationItem> Members

        public new IEnumerator<NamespaceConfigurationItem> GetEnumerator()
        {
            foreach (NamespaceConfigurationItem item in (this as IEnumerable))
            {
                yield return item;
            }
        }

        #endregion

        protected override ConfigurationElement CreateNewElement()
        {
            return new NamespaceConfigurationItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NamespaceConfigurationItem)element).NamespaceName;
        }
    }
}
