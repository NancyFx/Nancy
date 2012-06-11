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

        /// <summary>
        /// Merges a set of corsOptions from different modules into one. Very conservative atm.
        /// </summary>
        /// <param name="corsOptions">A list op CORSOptions objects that need to be merged</param>
        /// <returns></returns>
        public static CORSOptions MergeCORSOptions(IEnumerable<CORSOptions> corsOptions)
        {
            if (corsOptions.Count () == 1)
                return corsOptions.First();

            if (corsOptions.Count () == 0)
                return null;

            //should probably log a warning here when allowcredentials is set to false. when it's set to true on one of the modules.

            return new CORSOptions
            {
                AllowCredentials = corsOptions.All(o => o.AllowCredentials),
                DisalllowedHeaders = corsOptions.SelectMany(o => o.DisalllowedHeaders).ToList(),
                AllowedOrigins = corsOptions.SelectMany(o => o.AllowedOrigins).ToList()
            };
        }
    }
}