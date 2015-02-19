namespace Nancy.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Nancy.Bootstrapper;
    using Nancy.ViewEngines;

    /// <summary>
    /// Nancy configurable conventions
    /// </summary>
    public class NancyConventions
    {
        /// <summary>
        /// Discovered conventions
        /// </summary>
        private IEnumerable<IConvention> conventions;

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
        public IList<Func<string, dynamic, ViewLocationContext, string>> ViewLocationConventions { get; set; }

        /// <summary>
        /// Gets or sets the conventions for locating and serving static content
        /// </summary>
        public IList<Func<NancyContext, string, Response>> StaticContentsConventions { get; set; }

        /// <summary>
        /// Gets or sets the conventions for coercing accept headers from their source
        /// values to the real values for content negotiation
        /// <seealso cref="BuiltInAcceptHeaderCoercions"/>
        /// </summary>
        public IList<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>> AcceptHeaderCoercionConventions { get; set; }

        /// <summary>
        /// Gets or sets the conventions for determining request culture
        /// </summary>
        public IList<Func<NancyContext, CultureInfo>> CultureConventions { get; set; }

        /// <summary>
        /// Validates the conventions
        /// </summary>
        /// <returns>A tuple containing a flag indicating validity, and any error messages</returns>
        public Tuple<bool, string> Validate()
        {
            var isValid = true;
            var errorMessageBuilder = new StringBuilder();

            foreach (var result in this.conventions.Select(convention => convention.Validate(this)).Where(result => !result.Item1))
            {
                isValid = false;
                errorMessageBuilder.AppendLine(result.Item2);
            }

            return new Tuple<bool, string>(isValid, errorMessageBuilder.ToString());
        }

        /// <summary>
        /// Gets the instance registrations for registering into the container
        /// </summary>
        /// <returns>Enumeration of InstanceRegistration types</returns>
        public IEnumerable<InstanceRegistration> GetInstanceRegistrations()
        {
            return new[]
            {
                new InstanceRegistration(typeof(ViewLocationConventions), new ViewLocationConventions(this.ViewLocationConventions)),
                new InstanceRegistration(typeof(StaticContentsConventions), new StaticContentsConventions(this.StaticContentsConventions)),
                new InstanceRegistration(typeof(AcceptHeaderCoercionConventions), new AcceptHeaderCoercionConventions(this.AcceptHeaderCoercionConventions)), 
                new InstanceRegistration(typeof(CultureConventions), new CultureConventions(this.CultureConventions))
            };
        }

        /// <summary>
        /// Locates all the default conventions and calls them in
        /// turn to build up the default conventions.
        /// </summary>
        private void BuildDefaultConventions()
        {
            var defaultConventions =
                AppDomainAssemblyTypeScanner.TypesOf<IConvention>(ScanMode.OnlyNancy);

            this.conventions = defaultConventions
                .Union(AppDomainAssemblyTypeScanner.TypesOf<IConvention>(ScanMode.ExcludeNancy))
                .Select(t => (IConvention)Activator.CreateInstance(t));

            foreach (var convention in this.conventions)
            {
                convention.Initialise(this);
            }
        }
    }
}