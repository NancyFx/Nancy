using System.IO;

namespace Nancy.Routing.CORS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Extensions;

    /// <summary>
    /// Adds extensions to a module to give the use the ability to configure the automatic OPTIONS route for use with CORS.
    /// </summary>
    public static class CORSModuleExtension
    {
        private static readonly Dictionary<string, CORSOptions> Options = new Dictionary<string, CORSOptions>();

        private static Response ModifyOptionsResponse (NancyContext ctx, IEnumerable<string> allowedMethods, CORSOptions options)
        {
            var response = ctx.Response;
            var request = ctx.Request;

            if (request.Headers.AccessControlRequestMethod.Count () > 0)
            {
                response.Headers["Access-Control-Allow-Headers"] =
                    string.Join (", ", request.Headers.AccessControlRequestHeaders.Where (h => !options.DisalllowedHeaders.Contains (h)));

                if (options.AllowCredentials && options.AllowedOrigins.Contains ("*"))
                {
                    throw new CORSRequestException ("When allowing credentials the AllowedOrigins can't contain \"*\"");
                }

                if (options.AllowCredentials)
                {
                    response.Headers["Access-Control-Allow-Credentials"] = options.AllowCredentials.ToString ();
                }

                response.Headers["Access-Control-Allow-Methods"] = string.Join (", ", allowedMethods) + ", OPTIONS";

                response.Headers["Access-Control-Allow-Origin"] = string.Join (", ", options.AllowedOrigins);
            }

            return response;
        }

        /// <summary>
        /// Gives the user the ability to disable CORS.
        /// </summary>
        /// <param name="module"></param>
        public static void EnableCORS(this NancyModule module)
        {
            if (!Options.ContainsKey(module.GetModuleName()))
            {
                Options.Add(module.GetModuleName(), new CORSOptions());
            }

            module.After.AddItemToStartOfPipeline( c => ModifyOptionsResponse(c, module.Routes.Where(r => r.Description.Path == c.Request.Path).Select(
                                          r => r.Description.Method), Options[module.GetModuleName()]));
        }

        /// <summary>
        /// Extension method to give the user easy access to setting wether credentials are allowed in CORS requests
        /// </summary>
        /// <param name="module">the current Module</param>
        /// <param name="allow">A boolean specifying wether it is allowed or not. 
        /// Default as specified in https://developer.mozilla.org/en/http_access_control is false</param>
        public static void AllowCredentials(this NancyModule module, bool allow)
        {
            if (Options.ContainsKey (module.GetModuleName ()))
            {
                Options[module.GetModuleName ()].AllowCredentials = allow;
            }
            else
            {
                new CORSRequestException("CORS not enabled. Enable by calling this.EnableCORS() in the module");
            }
        }

        /// <summary>
        /// A list of Origins that are allowed to access the resource over CORS. Default is set to "*". 
        /// Note: When using AllowCredentials this must be changed.
        /// </summary>
        /// <param name="module">the current Module</param>
        /// <param name="allowedOrigins">A list of allowed origins</param>
        public static void AllowOrigins (this NancyModule module, params string [] allowedOrigins)
        {
            if (Options.ContainsKey (module.GetModuleName ()))
            {
                Options[module.GetModuleName ()].AllowedOrigins = allowedOrigins;
            }
            else
            {
                new CORSRequestException ("CORS not enabled. Enable by calling this.EnableCORS() in the module");
            }
        }

        /// <summary>
        /// A list of headers that aren't allowed to be used by CORS. Default all headers requested are allowed.
        /// </summary>
        /// <param name="module">the current Module</param>
        /// <param name="disallowedHeaders">A list of headers disallowed</param>
        public static void DisallowHeaders(this NancyModule module, params string [] disallowedHeaders)
        {
            if (Options.ContainsKey (module.GetModuleName ()))
            {
                Options[module.GetModuleName ()].DisalllowedHeaders = disallowedHeaders;
            }
            else
            {
                new CORSRequestException ("CORS not enabled. Enable by calling this.EnableCORS() in the module");
            }
        }

        public static bool ModuleHasCORSEnabled(this Tuple<string, int, RouteDescription, IRoutePatternMatchResult> route)
        {
            return Options.ContainsKey (route.Item1);
        }

    }
}