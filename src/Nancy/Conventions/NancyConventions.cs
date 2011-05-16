namespace Nancy.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Nancy configurable conventions
    /// </summary>
    public class NancyConventions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NancyConventions"/> class.
        /// </summary>
        public NancyConventions()
        {
            this.BuildDefaultConventions();
        }

        /// <summary>
        /// Gets or sets the conventions for locating view templates
        /// </summary>
        public IList<Func<string, dynamic, string, string>> ViewLocationConventions { get; set; }

        /// <summary>
        /// Locates all the default convention providers and calls them in
        /// turn to build up the default conventions.
        /// </summary>
        private void BuildDefaultConventions()
        {
            var defaultConventionProviders = AppDomainAssemblyTypeScanner
                                                .TypesOf<IDefaultConventionProvider>()
                                                .Select(t => (IDefaultConventionProvider)Activator.CreateInstance(t));

            foreach (var defaultConventionProvider in defaultConventionProviders)
            {
                defaultConventionProvider.Initialise(this);
            }
        }
    }
}