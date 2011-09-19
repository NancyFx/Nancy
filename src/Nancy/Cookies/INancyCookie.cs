namespace Nancy.Cookies
{
    using System;

    public interface INancyCookie
    {
        /// <summary>
        /// The domain to restrict the cookie to
        /// </summary>
        string Domain { get; set; }

        /// <summary>
        /// When the cookie should expire
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance containing the date and time when the cookie should expire; otherwise <see langword="null"/> if it should never expire.</value>
        DateTime? Expires { get; set; }

        /// <summary>
        /// The name of the cookie
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the encoded name of the cookie
        /// </summary>
        string EncodedName { get; }

        /// <summary>
        /// The path to restrict the cookie to
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// The value of the cookie
        /// </summary>
        string Value { get;  }

        /// <summary>
        /// Gets the encoded value of the cookie
        /// </summary>
        string EncodedValue { get; }

        /// <summary>
        /// Whether the cookie is http only
        /// </summary>
        bool HttpOnly { get; }

        /// <summary>
        /// Whether the cookie is secure (i.e. HTTPS only)
        /// </summary>
        bool Secure { get; }
    }
}