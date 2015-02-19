namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using global::Spark.Bindings;

    /// <summary>
    /// Loads binding files from the application path as returned by the current <see cref="IRootPathProvider"/>.
    /// </summary>
    /// <remarks>This will scan all sub-folders as well.</remarks>
    public class NancyBindingProvider : BindingProvider
    {
        private readonly IRootPathProvider rootPathProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyBindingProvider"/> class, 
        /// with the provided <paramref name="rootPathProvider"/>.
        /// </summary>
        /// <param name="rootPathProvider">The root path provider that defines where bindings should be looked for.</param>
        public NancyBindingProvider(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        public override IEnumerable<Binding> GetBindings(BindingRequest bindingRequest)
        {
            var locatedFiles = 
                Directory.GetFiles(this.rootPathProvider.GetRootPath(), "bindings.xml", SearchOption.AllDirectories);

            return locatedFiles.Any() ? 
                locatedFiles.SelectMany(this.LoadBindings) :
                Enumerable.Empty<Binding>();
        }

        private IEnumerable<Binding> LoadBindings(string fileName)
        {
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return this.LoadStandardMarkup(reader);
                    }
                }
            }
            catch (Exception)
            {
                return Enumerable.Empty<Binding>();
            }
        }
    }
}