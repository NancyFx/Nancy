namespace Nancy.Localization
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    /// <summary>
    /// Resx implementation of ITextResource
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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            culturedAssembly = assemblies.FirstOrDefault(x => x.GetManifestResourceNames().Any(y => y.Contains(".Resources.Text")));
            if (culturedAssembly != null)
            {
                resourceManager = new ResourceManager(culturedAssembly.GetName().Name + ".Resources.Text",
                                                      culturedAssembly);
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
            get
            {
                if (resourceManager == null)
                {
                    return null;
                }

                return resourceManager.GetString(key, context.Culture);
            }
        }
    }
}