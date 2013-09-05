namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using Nancy.Helpers;

    /// <summary>
    /// Defines the context that a <see cref="Browser"/> instance should run under.
    /// </summary>
    public class BrowserContext : IBrowserContextValues
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserContext"/> class.
        /// </summary>
        public BrowserContext()
        {
            this.Values.Headers = new Dictionary<string, IEnumerable<string>>();
            this.Values.Protocol = "http";
            this.Values.QueryString = String.Empty;
            this.Values.BodyString = String.Empty;
            this.Values.FormValues = String.Empty;
        }

        /// <summary>
        /// Gets or sets the that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="Stream"/> that contains the body that should be sent with the HTTP request.</value>
        Stream IBrowserContextValues.Body { get; set; }

        /// <summary>
        /// Gets or sets the protocol that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="string"/> contains the the protocol that should be sent with the HTTP request..</value>
        string IBrowserContextValues.Protocol { get; set; }

        /// <summary>
        /// Gets or sets the querystring
        /// </summary>
        string IBrowserContextValues.QueryString { get; set; }

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
            if (!String.IsNullOrEmpty(this.Values.BodyString))
            {
                throw new InvalidOperationException("Form value cannot be set as well as body string");
            }

            this.Values.FormValues += String.Format(
                "{0}{1}={2}",
                this.Values.FormValues.Length == 0 ? String.Empty : "&",
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
            this.Values.QueryString += String.Format(
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
        /// Sets the ClientCertificate
        /// </summary>
        /// <param name="certificate2">the certificate</param>
        public void ClientCertificate(X509Certificate2 certificate2)
        {
            this.Values.ClientCertificate = certificate2;
        }

        /// <summary>
        /// Sets the ClientCertificate
        /// </summary>
        /// <param name="certificate">the certificate in bytes</param>
        public void ClientCertificate(byte[] certificate)
        {
            this.Values.ClientCertificate = new X509Certificate2(certificate);
        }

        private IBrowserContextValues Values
        {
            get { return this; }
        }
    }
}