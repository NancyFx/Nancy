namespace Nancy.Localization
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Resource based implementation of <see cref="ITextResource"/>
    /// </summary>
    public class ResourceBasedTextResource  : ITextResource
    {
        private readonly Assembly culturedAssembly;
        private readonly ResourceManager resourceManager;

        /// <summary>
        /// Initializes a new instance of <see cref="ResourceBasedTextResource"/> to read strings from *.resx files
        /// </summary>
        /// <remarks>Looks for *.resx files in a Resources folder with files called Text.resx as default or Text.CultureName.resx eg/ Text.en-GB.resx</remarks>
        public ResourceBasedTextResource()
        {
            var assemblies = 
                AppDomain.CurrentDomain.GetAssemblies();

            this.culturedAssembly = 
                assemblies.FirstOrDefault(x => x.GetManifestResourceNames().Any(y => y.Contains(".Resources.Text")));

            if (this.culturedAssembly != null)
            {
                var baseName =
                    string.Concat(culturedAssembly.GetName().Name, ".Resources.Text");

                this.resourceManager = new ResourceManager(baseName, culturedAssembly);
            }
        }

        /// <summary>
        /// Used to return a string value from *.resx files
        /// </summary>
        /// <param name="key">The key to look for in the resource file</param>
        /// <param name="context">The NancyContext used to determine the culture for returning culture specific values</param>
        /// <returns>Returns a string value from culture specific or default file or null if key does not exist as determined by <see cref="ResourceManager"/> </returns>
        public string this[string key, NancyContext context]
        {
            get { return resourceManager == null ? null : resourceManager.GetString(key, context.Culture); }
        }
    }
}