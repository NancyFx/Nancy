namespace Nancy.ViewEngines
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    public class DefaultTextResource : ITextResource
    {
        private readonly Assembly culturedAssembly;
        private readonly ResourceManager resourceManager;

        public DefaultTextResource()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            culturedAssembly = assemblies.FirstOrDefault(x => x.GetManifestResourceNames().Any(y => y.Contains("Text.")));

            resourceManager = new ResourceManager(culturedAssembly.GetName().Name + ".Resources.Text", culturedAssembly);
        }

        public string this[string key, NancyContext context]
        {
            get
            {

                return resourceManager.GetString(key, context.Culture);
            }
        }
    }
}