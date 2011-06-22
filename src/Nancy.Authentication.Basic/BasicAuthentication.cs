using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Bootstrapper;
using Nancy.Security;

namespace Nancy.Authentication.Basic
{
    /// <summary>
    /// Nancy basic authentication implementation
    /// </summary>
    public static class BasicAuthentication
    {
        public const string Scheme = "Basic";

        /// <summary>
        /// Enables basic authentication for the application
        /// </summary>
        /// <param name="applicationPipelines">Pipelines to add handlers to (usually "this")</param>
        /// <param name="configuration">Forms authentication configuration</param>
        public static void Enable(IApplicationPipelines applicationPipelines, BasicAuthenticationConfiguration configuration)
        {
            if (applicationPipelines == null)
            {
                throw new ArgumentNullException("applicationPipelines");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            applicationPipelines.BeforeRequest.AddItemToStartOfPipeline(GetLoadAuthenticationHook(configuration));
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

            module.Before.AddItemToStartOfPipeline(GetLoadAuthenticationHook(configuration));
        }

        /// <summary>
        /// Gets the pre request hook for loading the authenticated user's details
        /// from the auth header.
        /// </summary>
        /// <param name="configuration">Basic authentication configuration to use</param>
        /// <returns>Pre request hook delegate</returns>
        private static Func<NancyContext, Response> GetLoadAuthenticationHook(BasicAuthenticationConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            return context =>
            {
                if (Authenticate(context, configuration))
                {
                    return null;
                }

                return context.Response;
            };
        }

        /// <summary>
        /// Gets the credentials from the request and tries to validate them against the configured user validator
        /// </summary>
        /// <param name="context">Nancy context</param>
        /// <param name="configuration">Configuration object</param>
        /// <returns>A value indicating whether the user was authenticated or not</returns>
        public static bool Authenticate(NancyContext context, BasicAuthenticationConfiguration configuration)
        {
            var credentials = ExtractCredentials(context.Request);

            if (credentials != null && credentials.Length == 2)
            {
                if (configuration.UserValidator.Validate(credentials[0], credentials[1]))
                {
                    context.Items[SecurityConventions.AuthenticatedUsernameKey] =
                        credentials[0];

                    return true;
                }
                else
                {
                    context.Response = new Response();
                    context.Response.StatusCode = HttpStatusCode.Unauthorized;

                    return false;
                }
            }
            else
            {
                context.Response = new Response();
                context.Response.StatusCode = HttpStatusCode.Unauthorized;
                context.Response.Headers.Add("WWW-Authenticate",
                    String.Format("{0} realm=\"{1}\"", Scheme, configuration.Realm));

                return false;
            }
        }

        private static string[] ExtractCredentials(Request request)
        {
            IEnumerable<string> values = null;

            if (!request.Headers.TryGetValue("Authorization", out values))
            {
                return null;
            }

            var authorization = values.FirstOrDefault();

            if (authorization != null && authorization.StartsWith(Scheme))
            {
                try
                {
                    string encodedUserPass = authorization.Substring(Scheme.Length).Trim();

                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string userPass = encoding.GetString(Convert.FromBase64String(encodedUserPass));
                    int separator = userPass.IndexOf(':');

                    string[] credentials = new string[2];
                    credentials[0] = userPass.Substring(0, separator);
                    credentials[1] = userPass.Substring(separator + 1);

                    return credentials;
                }
                catch (FormatException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
