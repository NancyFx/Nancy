namespace Nancy.Culture
{
    using System.Globalization;

    using Nancy.Conventions;

    /// <summary>
    /// Determines current culture for context
    /// </summary>
    public class DefaultCultureService : ICultureService
    {
        private readonly CultureConventions cultureConventions;

        /// <summary>
        /// Creates a new instance of DefaultCultureService
        /// </summary>
        /// <param name="cultureConventions">CultureConventions to use for determining culture</param>
        public DefaultCultureService(CultureConventions cultureConventions)
        {
            this.cultureConventions = cultureConventions;
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
                culture = convention(context);
                if (culture != null)
                {
                    break;
                }
            }

            return culture;
        }
    }
}
