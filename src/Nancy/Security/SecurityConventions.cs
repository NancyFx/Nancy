namespace Nancy.Security
{
    /// <summary>
    /// Security filter conventions
    /// </summary>
    public static class SecurityConventions
    {
        /// <summary>
        /// Context key that contains the username
        /// </summary>
        private static string authenticatedUsernameKey = "username";

        /// <summary>
        /// Context key containing the current user's claims
        /// </summary>
        private static string authenticatedClaimsKey = "claims";

        /// <summary>
        /// Gets or sets the context key that contains the authenticated username
        /// </summary>
        public static string AuthenticatedUsernameKey 
        { 
            get
            {
                return authenticatedUsernameKey;
            }

            set
            {
                authenticatedUsernameKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the context key that contains the authenticated user's claims
        /// </summary>
        public static string AuthenticatedClaimsKey
        {
            get
            {
                return authenticatedClaimsKey;
            }

            set
            {
                authenticatedClaimsKey = value;
            }
        }
    }
}