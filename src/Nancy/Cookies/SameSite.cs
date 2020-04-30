namespace Nancy.Cookies
{
    /// <summary>
    /// Represents the SameSite for NancyCookie
    /// </summary>
    public enum SameSite
    {
        /// <summary>
        /// If the value is invalid, the cookie will only be sent along with
        /// "same-site" requests.  If the value is "Lax", the cookie will be
        /// sent with "same-site" requests, and with "cross-site" top-level navigations
        /// </summary>
        Lax = 0,

        /// <summary>
        /// If you set SameSite to Strict, your cookie will only be sent in a
        /// first-party context. In user terms, the cookie will only be sent
        /// if the site for the cookie matches the site currently shown in
        /// the browser's URL bar.
        /// </summary>
        Strict,

        /// <summary>
        /// Cookies with SameSite=None must also specify Secure,
        /// meaning they require a secure context.
        /// </summary>
        None
    }
}
