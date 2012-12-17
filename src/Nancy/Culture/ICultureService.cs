namespace Nancy.Culture
{
    using System.Globalization;

    /// <summary>
    /// Provides current culture for Nancy context
    /// </summary>
    public interface ICultureService
    {
        /// <summary>
        /// Determine current culture for NancyContext
        /// </summary>
        /// <param name="context">NancyContext</param>
        /// <returns>CultureInfo</returns>
        CultureInfo DetermineCurrentCulture(NancyContext context);
    }
}
