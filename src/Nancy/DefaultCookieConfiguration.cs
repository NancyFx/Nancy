namespace Nancy
{
    using Nancy.Cookies;

    /// <summary>
    /// Default cookie configuration with default name "_nc"
    /// </summary>
    public class DefaultCookieConfiguration:ICookieConfiguration
    {
        internal const string DefaultCookieName = "_nc";

        /// <summary>
        /// Cookie name for storing session information
        /// </summary>
        public string CookieName { get; set; }

        /// <summary>
        /// Gets or sets the domain of the session cookie
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path of the session cookie
        /// </summary>
        public string Path { get; set; }

        public DefaultCookieConfiguration()
        {
            this.CookieName = DefaultCookieName;
        }
    }
}
