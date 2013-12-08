namespace Nancy
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    /// <summary>
    /// Represents a full Url of the form scheme://hostname:port/basepath/path?query#fragment
    /// </summary>
    public sealed class Url : ICloneable
    {
        private string basePath;

        /// <summary>
        /// Represents a URL made up of component parts
        /// </summary>
        public Url()
        {
            this.Scheme = "http";
            this.HostName = String.Empty;
            this.Port = null;
            this.BasePath = String.Empty;
            this.Path = String.Empty;
            this.Query = String.Empty;
            this.Fragment = String.Empty;
        }

        /// <summary>
        /// Gets or sets the HTTP protocol used by the client.
        /// </summary>
        /// <value>The protocol.</value>
        public string Scheme { get; set; }

        /// <summary>
        /// Gets the hostname of the request
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Gets the port name of the request
        /// </summary>
        public int? Port { get; set; }

        /// <summary>
        /// Gets the base path of the request i.e. the "Nancy root"
        /// </summary>
        public string BasePath
        {
            get { return this.basePath; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                this.basePath = value.TrimEnd('/');
            }
        }

        /// <summary>
        /// Gets the path of the request, relative to the base path
        /// This property drives the route matching
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets the querystring data of the requested resource.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Gets the fragment of the request
        /// </summary>
        public string Fragment { get; set; }

        /// <summary>
        /// Gets the domain part of the request
        /// </summary>
        public string SiteBase
        {
            get
            {
                return this.Scheme + "://" +
                       GetHostName(this.HostName) +
                       GetPort(this.Port);
            }
        }

        /// <summary>
        /// Gets whether the url is secure or not.
        /// </summary>
        public bool IsSecure
        {
            get
            {
                return "https".Equals(this.Scheme, StringComparison.OrdinalIgnoreCase);
            }
        }

        public override string ToString()
        {
            return this.Scheme + "://" + 
                GetHostName(this.HostName) + 
                GetPort(this.Port) +
                GetCorrectPath(this.BasePath) +
                GetCorrectPath(this.Path) +
                GetQuery(this.Query) +
                GetFragment(this.Fragment);
        }

        private static string GetQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return string.Empty;
            }

            return query.StartsWith("?", StringComparison.OrdinalIgnoreCase) ?
                query :
                string.Concat("?", query);
        }

        /// <summary>
        /// Clones the url.
        /// </summary>
        /// <returns>Returns a new cloned instance of the url.</returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Clones the url.
        /// </summary>
        /// <returns>Returns a new cloned instance of the url.</returns>
        public Url Clone()
        {
            return new Url
                       {
                           BasePath = this.BasePath,
                           Fragment = this.Fragment,
                           HostName = this.HostName,
                           Port = this.Port,
                           Query = this.Query,
                           Path = this.Path,
                           Scheme = this.Scheme
                       };
        }

        /// <summary>
        /// Casts the current <see cref="Url"/> instance to a <see cref="Uri"/> instance.
        /// </summary>
        /// <param name="url">The instance that should be cast.</param>
        /// <returns>An <see cref="Uri"/> representation of the <paramref name="url"/>.</returns>
        public static implicit operator Uri(Url url)
        {
            return new Uri(url.ToString(), UriKind.Absolute);
        }

        /// <summary>
        /// Casts a <see cref="Uri"/> instance to a <see cref="Url"/> instance
        /// </summary>
        /// <param name="uri">The instance that should be cast.</param>
        /// <returns>An <see cref="Url"/> representation of the <paramref name="uri"/>.</returns>
        public static implicit operator Url(Uri uri)
        {
            var url = new Url
            {
                HostName = uri.Host,
                Path = uri.LocalPath,
                Port = uri.Port,
                Query = uri.Query,
                Scheme = uri.Scheme
            };

            return url;
        }

        private static string GetFragment(string fragment)
        {
            return (string.IsNullOrEmpty(fragment)) ? string.Empty : string.Concat("#", fragment);
        }

        private static string GetCorrectPath(string path)
        {
            return (string.IsNullOrEmpty(path) || path.Equals("/")) ? string.Empty : path;
        }

        private static string GetPort(int? port)
        {
            return (!port.HasValue) ?
                string.Empty : 
                string.Concat(":", port.Value);
        }

        private static string GetHostName(string hostName)
        {
            IPAddress address;

            if (IPAddress.TryParse(hostName, out address))
            {
                return (address.AddressFamily == AddressFamily.InterNetworkV6)
                           ? string.Concat("[", address.ToString(), "]")
                           : address.ToString();

            }

            return hostName;
        }
    }
}