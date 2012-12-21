namespace Nancy.ViewEngines
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    public class DefaultTextResource : ITextResource
    {
        private readonly Assembly culturedAssembly;

        public DefaultTextResource()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            culturedAssembly = assemblies.FirstOrDefault(x => x.GetManifestResourceNames().Any(y => y.Contains("Text.")));
        }

        public string this[string key, NancyContext context]
        {
            get
            {
                var manager = new ResourceManager(culturedAssembly.GetName().Name + ".Resources.Text", culturedAssembly);
                return manager.GetString(key, context.Culture);
            }
        }
    }
}