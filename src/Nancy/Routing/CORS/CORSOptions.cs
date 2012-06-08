namespace Nancy.Routing.CORS
{
    using System.Linq;
    using System.Collections.Generic;

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