namespace Nancy.Owin
{
    using System.Collections.Generic;

    /// <summary>
    /// OWIN extensions for the NancyContext.
    /// </summary>
    public static class NancyContextExtensions
    {
        /// <summary>
        /// Gets the OWIN environment dictionary.
        /// </summary>
        /// <param name="context">The Nancy context.</param>
        /// <returns>The OWIN environment dictionary.</returns>
        public static IDictionary<string, object> GetOwinEnvironment(this NancyContext context)
        {
            object environment;
            if (context.Items.TryGetValue(NancyOwinHost.RequestEnvironmentKey, out environment))
            {
                return environment as IDictionary<string, object>;
            }

            return null;
        }
    }
}