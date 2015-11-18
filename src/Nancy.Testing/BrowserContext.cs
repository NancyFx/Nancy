namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using Configuration;
    using Nancy.Helpers;

    /// <summary>
    /// Defines the context that a <see cref="Browser"/> instance should run under.
    /// </summary>
    public class BrowserContext : IBrowserContextValues
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserContext"/> class,
        /// with the provided <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public BrowserContext(INancyEnvironment environment)
        {
            this.Environment = environment;
            this.Values.Headers = new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
            this.Values.Protocol = string.Empty;
            this.Values.QueryString = string.Empty;
            this.Values.BodyString = string.Empty;
            this.Values.FormValues = string.Empty;
            this.Values.HostName = string.Empty;
        }

        /// <summary>
        /// Gets the <see cref="INancyEnvironment"/> instance used by the <see cref="Browser"/>.
        /// </summary>
        /// <value>An <see cref="INancyEnvironment"/> instance.</value>
        public INancyEnvironment Environment { get; private set; }

        /// <summary>
        /// Gets or sets the that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="Stream"/> that contains the body that should be sent with the HTTP request.</value>
        Stream IBrowserContextValues.Body { get; set; }

        /// <summary>
        /// Gets or sets the protocol that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="string"/> contains the protocol that should be sent with the HTTP request..</value>
        string IBrowserContextValues.Protocol { get; set; }

        /// <summary>
        /// Gets or sets the querystring
        /// </summary>
        string IBrowserContextValues.QueryString { get; set; }

        /// <summary>
        /// Gets or sets the user host name
        /// </summary>
        string IBrowserContextValues.HostName { get; set; }

        /// <summary>
        /// Gets or sets the user host address
        /// </summary>
        string IBrowserContextValues.UserHostAddress { get; set; }

        /// <summary>
        /// Gets or sets the ClientCertificate
        /// </summary>
        X509Certificate2 IBrowserContextValues.ClientCertificate { get; set; }

        /// <summary>
        /// Gets or sets the body string
        /// </summary>
        string IBrowserContextValues.BodyString { get; set; }

        /// <summary>
        /// Gets or sets the form values string
        /// </summary>
        string IBrowserContextValues.FormValues { get; set; }

        /// <summary>
        /// Gets or sets the headers that should be sent with the HTTP request.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance that contains the headers that should be sent with the HTTP request.</value>
        IDictionary<string, IEnumerable<string>> IBrowserContextValues.Headers { get; set; }

        /// <summary>
        /// Adds a body to the HTTP request.
        /// </summary>
        /// <param name="body">A string that should be used as the HTTP request body.</param>
        public void Body(string body)
        {
            this.Values.BodyString = body;
        }

        /// <summary>
        /// Adds a body to the HTTP request.
        /// </summary>
        /// <param name="body">A string that should be used as the HTTP request body.</param>
        /// <param name="contentType">Content type of the HTTP request body.</param>
        public void Body(string body, string contentType)
        {
            this.Values.BodyString = body;
            this.Header("Content-Type", contentType);
        }

        /// <summary>
        /// Adds a body to the HTTP request.
        /// </summary>
        /// <param name="body">A stream that should be used as the HTTP request body.</param>
        /// <param name="contentType">Content type of the HTTP request body. Defaults to 'application/octet-stream'</param>
        public void Body(Stream body, string contentType = null)
        {
            this.Values.Body = body;
            this.Header("Content-Type", contentType ?? "application/octet-stream");
        }

        /// <summary>
        /// Adds an application/x-www-form-urlencoded form value.
        /// </summary>
        /// <param name="key">The name of the form element.</param>
        /// <param name="value">The value of the form element.</param>
        public void FormValue(string key, string value)
        {
            if (!string.IsNullOrEmpty(this.Values.BodyString))
            {
                throw new InvalidOperationException("Form value cannot be set as well as body string");
            }

            this.Values.FormValues += string.Format(
                "{0}{1}={2}",
                this.Values.FormValues.Length == 0 ? string.Empty : "&",
                key,
                HttpUtility.UrlEncode(value));
        }

        /// <summary>
        /// Adds a header to the HTTP request.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        public void Header(string name, string value)
        {
            if (!this.Values.Headers.ContainsKey(name))
            {
                this.Values.Headers.Add(name, new List<string>());
            }

            var values = (List<string>)this.Values.Headers[name];
            values.Add(value);

            this.Values.Headers[name] = values;
        }

        /// <summary>
        /// Configures the request to be sent over HTTP.
        /// </summary>
        public void HttpRequest()
        {
            this.Values.Protocol = "http";
        }

        /// <summary>
        /// Configures the request to be sent over HTTPS.
        /// </summary>
        public void HttpsRequest()
        {
            this.Values.Protocol = "https";
        }

        /// <summary>
        /// Adds a query string entry
        /// </summary>
        public void Query(string key, string value)
        {
            this.Values.QueryString += string.Format(
                "{0}{1}={2}",
                this.Values.QueryString.Length == 0 ? "?" : "&",
                key,
                HttpUtility.UrlEncode(value));
        }

        /// <summary>
        /// Sets the user host address.
        /// </summary>
        public void UserHostAddress(string userHostAddress)
        {
            this.Values.UserHostAddress = userHostAddress;
        }

        /// <summary>
        /// Sets the host name.
        /// </summary>
        /// <param name="hostName">is the host name of request url string</param>
        public void HostName(string hostName)
        {
            this.Values.HostName = hostName;
        }

        /// <summary>
        /// Sets the ClientCertificate to a default embedded certificate
        /// <remarks>The default certificate is embedded using the Nancy.Testing.Nancy Testing Cert.pfx resource name (secured with password "nancy")</remarks>
        /// </summary>
        public void Certificate()
        {
            X509Certificate2 certificate2;

            using (
                var pkcs12 =
                    Assembly.GetAssembly(typeof (BrowserContext))
                            .GetManifestResourceStream("Nancy.Testing.Resources.Nancy Testing Cert.pfx"))
            {
                using (var br = new BinaryReader(pkcs12))
                {
                    certificate2 = new X509Certificate2(br.ReadBytes((int)pkcs12.Length), "nancy",
                                                        X509KeyStorageFlags.Exportable);
                }
            }

            this.Values.ClientCertificate = certificate2;
        }

        /// <summary>
        /// Sets the ClientCertificate
        /// </summary>
        /// <param name="certificate">the certificate in bytes</param>
        public void Certificate(byte[] certificate)
        {
            this.Values.ClientCertificate = new X509Certificate2(certificate);
        }

        /// <summary>
        /// Sets the ClientCertificate
        /// </summary>
        /// <param name="certificate">the certificate</param>
        public void Certificate(X509Certificate2 certificate)
        {
            this.Values.ClientCertificate = certificate;
        }

        /// <summary>
        /// Find a certificate in a store on the computer.
        /// </summary>
        /// <param name="storeLocation">The location of the store (LocalMachine, CurrentUser)</param>
        /// <param name="storeName">The name of the store (e.q. My)</param>
        /// <param name="findType">By which field you want to find the certificate (Commonname, Thumbprint, etc)</param>
        /// <param name="findBy">The "Common name" or "thumbprint" you are looking for</param>
        public void Certificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findBy)
        {
            var store = new X509Store(storeName, storeLocation);

            store.Open(OpenFlags.ReadOnly);
            var certificatesFound = store.Certificates.Find(findType, findBy, false);

            if (certificatesFound.Count <= 0)
            {
                throw new InvalidOperationException(string.Format("No certificates found in {0} {1} with a {2} that looks like \"{3}\"", storeLocation, storeName, findType, findBy));
            }

            this.Values.ClientCertificate = certificatesFound[0];
        }

        private IBrowserContextValues Values
        {
            get { return this; }
        }
    }
}