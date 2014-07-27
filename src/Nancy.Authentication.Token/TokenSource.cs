namespace Nancy.Authentication.Token
{
    /// <summary>
    /// Represents where to get the auth token from
    /// </summary>
    public enum TokenSource
    {
        /// <summary>
        /// Use Authorization header
        /// </summary>
        Header,
        /// <summary>
        /// User Authorization query parameter
        /// </summary>
        Query
    }
}