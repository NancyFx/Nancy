namespace Nancy.Routing.CORS
{
    /// <summary>
    /// Adds extensions to a module to give the use the ability to configure the automatic OPTIONS route for use with CORS.
    /// </summary>
    public static class ModuleExtensions
    {
        /// <summary>
        /// Gives the user the ability to disable CORS.
        /// </summary>
        /// <param name="module"></param>
        /// <param name="disabled"></param>
        public static void DisableCORS(this NancyModule module, bool disabled)
        {
            OptionsRoute.DisableCORS = true;
        }

        /// <summary>
        /// Extension method to give the user easy access to setting wether credentials are allowed in CORS requests
        /// </summary>
        /// <param name="module">the current Module</param>
        /// <param name="allow">A boolean specifying wether it is allowed or not. 
        /// Default as specified in https://developer.mozilla.org/en/http_access_control is false</param>
        public static void AllowCredentials(this NancyModule module, bool allow)
        {
            OptionsRoute.AllowCredentials = allow;
        }

        /// <summary>
        /// A list of Origins that are allowed to access the resource over CORS. Default is set to "*". 
        /// Note: When using AllowCredentials this must be changed.
        /// </summary>
        /// <param name="module">the current Module</param>
        /// <param name="allowedOrigins">A list of allowed origins</param>
        public static void AllowOrigins (this NancyModule module, params string [] allowedOrigins)
        {
            OptionsRoute.AllowedOrigins = allowedOrigins;
        }

        /// <summary>
        /// A list of headers that aren't allowed to be used by CORS. Default all headers requested are allowed.
        /// </summary>
        /// <param name="module">the current Module</param>
        /// <param name="disllowedHeaders">A list of headers disallowed</param>
        public static void DisallowHeaders(this NancyModule module, params string [] disllowedHeaders)
        {
            OptionsRoute.DisalllowedHeaders = disllowedHeaders;
        }
    }
}