namespace Nancy.Authentication.Basic
{
    using System;
using System.Text;
using Nancy.Bootstrapper;
    using Nancy.Extensions;
using Nancy.Security;

    /// <summary>
    /// Nancy basic authentication implementation
    /// </summary>
    public static class BasicAuthentication
    {
        private const string SCHEME = "Basic";

        /// <summary>
        /// Enables basic authentication for the application
        /// </summary>
        /// <param name="pipelines">Pipelines to add handlers to (usually "this")</param>
        /// <param name="configuration">Forms authentication configuration</param>
        public static void Enable(IPipelines pipelines, BasicAuthenticationConfiguration configuration)
        {
            if (pipelines == null)
            {
                throw new ArgumentNullException("pipelines");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            pipelines.BeforeRequest.AddItemToStartOfPipeline(GetCredentialRetrievalHook(configuration));
            pipelines.AfterRequest.AddItemToEndOfPipeline(GetAuthenticationPromptHook(configuration));
        }

        /// <summary>
        /// Enables basic authentication for a module
        /// </summary>
        /// <param name="module">Module to add handlers to (usually "this")</param>
        /// <param name="configuration">Forms authentication configuration</param>
        public static void Enable(NancyModule module, BasicAuthenticationConfiguration configuration)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            module.RequiresAuthentication();
            module.Before.AddItemToStartOfPipeline(GetCredentialRetrievalHook(configuration));
            module.After.AddItemToEndOfPipeline(GetAuthenticationPromptHook(configuration));
        }

        /// <summary>
        /// Gets the pre request hook for loading the authenticated user's details
        /// from the auth header.
        /// </summary>
        /// <param name="configuration">Basic authentication configuration to use</param>
        /// <returns>Pre request hook delegate</returns>
        private static Func<NancyContext, Response> GetCredentialRetrievalHook(BasicAuthenticationConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            return context =>
                {
                    RetrieveCredentials(context, configuration);
                    return null;
                };
        }

        private static Action<NancyContext> GetAuthenticationPromptHook(BasicAuthenticationConfiguration configuration)
        {
            return context =>
                {
                    if (context.Response.StatusCode == HttpStatusCode.Unauthorized && SendAuthenticateResponseHeader(context, configuration))
                    {
                        context.Response.Headers["WWW-Authenticate"] = String.Format("{0} realm=\"{1}\"", SCHEME, configuration.Realm);
                    }
                };
        }

        private static void RetrieveCredentials(NancyContext context, BasicAuthenticationConfiguration configuration)
        {
            var credentials = 
                ExtractCredentialsFromHeaders(context.Request);

            if (credentials != null && credentials.Length == 2)
            {
                var user = configuration.UserValidator.Validate(credentials[0], credentials[1]);

                if (user != null)
                {
                    context.CurrentUser = user;
                }
            }
        }

        private static string[] ExtractCredentialsFromHeaders(Request request)
        {
            var authorization =
                request.Headers.Authorization;

            if (string.IsNullOrEmpty(authorization))
            {
                return null;
            }

            if (!authorization.StartsWith(SCHEME))
            {
                return null;
            }

            try
            {
                var encodedUserPass = authorization.Substring(SCHEME.Length).Trim();
                var userPass = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUserPass));

                return String.IsNullOrWhiteSpace(userPass) ? null : userPass.Split(':');
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private static bool SendAuthenticateResponseHeader(NancyContext context, BasicAuthenticationConfiguration configuration)
        {
            return configuration.UserPromptBehaviour == UserPromptBehaviour.Always || (configuration.UserPromptBehaviour == UserPromptBehaviour.NonAjax && !context.Request.IsAjaxRequest());
        }
    }
}
