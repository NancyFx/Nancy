namespace Nancy.Culture
{
    using System.Globalization;
    using Nancy.Configuration;
    using Nancy.Conventions;

    /// <summary>
    /// Determines current culture for context
    /// </summary>
    public class DefaultCultureService : ICultureService
    {
        private readonly CultureConventions cultureConventions;
        private readonly GlobalizationConfiguration configuration;

        /// <summary>
        /// Creates a new instance of DefaultCultureService
        /// </summary>
        /// <param name="cultureConventions">CultureConventions to use for determining culture</param>
        /// <param name="environment">An instance of <see cref="INancyEnvironment"/> to retrieve <see cref="GlobalizationConfiguration"/> from.</param>
        public DefaultCultureService(CultureConventions cultureConventions, INancyEnvironment environment)
        {
            this.cultureConventions = cultureConventions;
            this.configuration = environment.GetValue<GlobalizationConfiguration>();
        }

        /// <summary>
        /// Determine current culture for NancyContext
        /// </summary>
        /// <param name="context">NancyContext</param>
        /// <returns>CultureInfo</returns>
        public CultureInfo DetermineCurrentCulture(NancyContext context)
        {
            CultureInfo culture = null;

            foreach (var convention in this.cultureConventions)
            {
                culture = convention(context, this.configuration);
                if (culture != null)
                {
                    break;
                }
            }

            return culture;
        }
    }
}
