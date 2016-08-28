namespace Nancy.Cookies
{
    using System;
    using System.Globalization;
    using System.Text;

    using Nancy.Helpers;

    /// <summary>
    /// Default cookie implementation for Nancy.
    /// </summary>
    public class NancyCookie : INancyCookie
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCookie"/> class.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        public NancyCookie(string name, string value)
            : this(name, value, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCookie"/> class.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="expires">The expiration date of the cookie. Can be <see langword="null" /> if it should expire at the end of the session.</param>
        public NancyCookie(string name, string value, DateTime expires)
            : this(name, value, false, false, expires)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCookie"/> class.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="httpOnly">Whether the cookie is http only.</param>
        public NancyCookie(string name, string value, bool httpOnly)
            : this(name, value, httpOnly, false, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCookie"/> class.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="httpOnly">Whether the cookie is http only.</param>
        /// <param name="secure">Whether the cookie is secure (i.e. HTTPS only).</param>
        public NancyCookie(string name, string value, bool httpOnly, bool secure)
            : this(name, value, httpOnly, secure, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyCookie"/> class.
        /// </summary>
        /// <param name="name">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        /// <param name="httpOnly">Whether the cookie is http only.</param>
        /// <param name="secure">Whether the cookie is secure (i.e. HTTPS only).</param>
        /// <param name="expires">The expiration date of the cookie. Can be <see langword="null" /> if it should expire at the end of the session.</param>
        public NancyCookie(string name, string value, bool httpOnly, bool secure, DateTime? expires)
        {
            this.Name = name;
            this.Value = value;
            this.HttpOnly = httpOnly;
            this.Secure = secure;
            this.Expires = expires;
        }

        /// <summary>
        /// The domain to restrict the cookie to
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// When the cookie should expire
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance containing the date and time when the cookie should expire; otherwise <see langword="null"/> if it should expire at the end of the session.</value>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// The name of the cookie
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the encoded name of the cookie
        /// </summary>
        public string EncodedName
        {
            get
            {
                return HttpUtility.UrlEncode(this.Name);
            }
        }

        /// <summary>
        /// The path to restrict the cookie to
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The value of the cookie
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the encoded value of the cookie
        /// </summary>
        public string EncodedValue
        {
            get
            {
                return HttpUtility.UrlEncode(this.Value);
            }
        }

        /// <summary>
        /// Whether the cookie is http only
        /// </summary>
        public bool HttpOnly { get; private set; }

        /// <summary>
        /// Whether the cookie is secure (i.e. HTTPS only)
        /// </summary>
        public bool Secure { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder(50);
            sb.AppendFormat("{0}={1}; path={2}", this.EncodedName, this.EncodedValue, Path ?? "/");

            if (Expires != null)
            {
                sb.Append("; expires=");
                sb.Append(Expires.Value.ToUniversalTime().ToString("ddd, dd-MMM-yyyy HH:mm:ss", DateTimeFormatInfo.InvariantInfo));
                sb.Append(" GMT");
            }

            if (Domain != null)
            {
                sb.Append("; domain=");
                sb.Append(Domain);
            }

            if (Secure)
            {
                sb.Append("; Secure");
            }

            if (HttpOnly)
            {
                sb.Append("; HttpOnly");
            }

            return sb.ToString();
        }
    }
}