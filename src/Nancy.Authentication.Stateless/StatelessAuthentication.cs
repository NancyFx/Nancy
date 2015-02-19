namespace Nancy.Authentication.Stateless
{
    using System;

    using Nancy.Bootstrapper;

    /// <summary>
    /// Nancy stateless authentication implementation
    /// </summary>
    public static class StatelessAuthentication
    {
        /// <summary>
        /// Enables stateless authentication for the application
        /// </summary>
        /// <param name="pipelines">Pipelines to add handlers to (usually "this")</param>
        /// <param name="configuration">Stateless authentication configuration</param>
        public static void Enable(IPipelines pipelines, StatelessAuthenticationConfiguration configuration)
        {
            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (!configuration.IsValid)
            {
                throw new ArgumentException("Configuration is invalid", "configuration");
            }

            pipelines.BeforeRequest.AddItemToStartOfPipeline(GetLoadAuthenticationHook(configuration));
        }

        /// <summary>
        /// Enables stateless authentication for a module
        /// </summary>
        /// <param name="module">Module to add handlers to (usually "this")</param>
        /// <param name="configuration">Stateless authentication configuration</param>
        public static void Enable(INancyModule module, StatelessAuthenticationConfiguration configuration)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            if (!configuration.IsValid)
            {
                throw new ArgumentException("Configuration is invalid", "configuration");
            }

            module.Before.AddItemToStartOfPipeline(GetLoadAuthenticationHook(configuration));
        }

        /// <summary>
        /// Gets the pre request hook for loading the authenticated user's details
        /// from apikey given in request.
        /// </summary>
        /// <param name="configuration">Stateless authentication configuration to use</param>
        /// <returns>Pre request hook delegate</returns>
        static Func<NancyContext, Response> GetLoadAuthenticationHook(StatelessAuthenticationConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            return context =>
            {
                try
                {
                    context.CurrentUser = configuration.GetUserIdentity(context);
                    return context.Response;
                }
                catch (Exception)
                {
                    return context.Response;
                }
            };
        }
    }
}