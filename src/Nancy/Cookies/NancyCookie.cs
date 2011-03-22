namespace Nancy.Cookies
{
    using System;
    using System.Globalization;
    using System.Text;

    using Nancy.Helpers;

    public class NancyCookie : INancyCookie
    {
        public NancyCookie(string name, string value)
            : this(name, value, false)
        {
        }

        public NancyCookie(string name, string value, bool httpOnly)
        {
            this.Name = name;
            this.Value = value;
            this.HttpOnly = httpOnly;
        }

        /// <summary>
        /// The domain to restrict the cookie to
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// When the cookie should expire
        /// </summary>
        /// <value>A <see cref="DateTime"/> instance containing the date and time when the cookie should expire; otherwise <see langword="null"/> if it should never expire.</value>
        public DateTime? Expires { get; set; }

        /// <summary>
        /// The name of the cookie
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The path to restrict the cookie to
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The value of the cookie
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Whether the cookie is http only
        /// </summary>
        public bool HttpOnly { get; private set; }
        
        public override string ToString()
        {
            var sb = new StringBuilder(50);
            sb.AppendFormat("{0}={1}; path={2}", HttpUtility.UrlEncode(Name), HttpUtility.UrlEncode(Value), Path ?? "/");
            
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

            if (HttpOnly)
            {
                sb.Append("; HttpOnly");
            }

            return sb.ToString();
        }
    }
}