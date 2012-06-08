namespace Nancy.Routing.CORS
{
    using System.Linq;
    using System.Collections.Generic;

    using Nancy.Extensions;
    
    /// <summary>
    /// Adds extensions to a module to give the use the ability to configure the automatic OPTIONS route for use with CORS.
    /// </summary>
    public static class CORSModuleExtensions
    {
        private static readonly Dictionary<string, CORSOptions> Options = new Dictionary<string, CORSOptions>();

        private static Response CreateMethodOptionsResponse (IEnumerable<string> allowedMethods, Request request, CORSOptions options)
        {
            var response = new Response ();
            
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
            response.StatusCode = HttpStatusCode.OK;

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

            module.Before.AddItemToStartOfPipeline(c =>
            {
                if (c.Request.Method == "OPTIONS")
                {
                    return CreateMethodOptionsResponse(
                        module.Routes.Where(r => r.Description.Path == c.Request.Path).Select(r => r.Description.Method),
                        c.Request, Options[module.GetModuleName ()]);
                }
                
                return new Response ();
            });
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
    }

    public class CORSOptions
    {
        /// <summary>
        /// Allows the user to determine wether or not he wants to enable cookies over CORS.
        /// </summary>
        public bool AllowCredentials { get; set; }

        private IEnumerable<string> _allowedOrigins;

        /// <summary>
        /// Gives the user the ability to specify allowedorigins for CORS requests.
        /// </summary>
        public IEnumerable<string> AllowedOrigins
        {
            get { return _allowedOrigins ?? new[] { "*" }; }
            set { _allowedOrigins = value; }
        }

        private IEnumerable<string> _disalllowedHeaders;

        /// <summary>
        /// Gives the user the ability to specify the headers that aren't allowed, by default all requested are returned.
        /// </summary>
        public IEnumerable<string> DisalllowedHeaders
        {
            get { return _disalllowedHeaders ?? Enumerable.Empty<string> (); }
            set { _disalllowedHeaders = value; }
        }
    }
}