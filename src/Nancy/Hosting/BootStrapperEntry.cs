namespace Nancy.Hosting
{
    using System.Web;
    using Routing;
    using System;
    using Nancy.BootStrapper;
    using System.Configuration;

    public sealed class BootStrapperEntry
    {
        public string Assembly { get; private set; }
        public string Name { get; private set; }

        public BootStrapperEntry(string assembly, string name)
        {
            Assembly = assembly;
            Name = name;
        }
    }
}
