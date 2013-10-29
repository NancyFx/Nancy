namespace Nancy.Testing
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    /// Provides an API ontop of <see cref="BrowserContext"/> for extracting values.
    /// </summary>
    public interface IBrowserContextValues : IHideObjectMembers
    {
        /// <summary>
        /// Gets or sets the stream that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="Stream"/> that contains the body that should be sent with the HTTP request.</value>
        Stream Body { get; set; }

        /// <summary>
        /// Gets or sets the body string
        /// </summary>
        string BodyString { get; set; }

        /// <summary>
        /// Gets or sets the form values string
        /// </summary>
        /// <remarks>If <see cref="BodyString"/> is assigned a value, the <see cref="FormValues"/> will be ignored.</remarks>
        string FormValues { get; set; }

        /// <summary>
        /// Gets or sets the headers that should be sent with the HTTP request.
        /// </summary>
        /// <value>An <see cref="IDictionary{TKey,TValue}"/> instance that contains the headers that should be sent with the HTTP request.</value>
        IDictionary<string, IEnumerable<string>> Headers { get; set; }

        /// <summary>
        /// Gets or sets the protocol that should be sent with the HTTP request.
        /// </summary>
        /// <value>A <see cref="string"/> contains the the protocol that should be sent with the HTTP request.</value>
        string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the querystring
        /// </summary>
        string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the user host address
        /// </summary>
        string UserHostAddress { get; set; }

        /// <summary>
        /// Gets or sets the ClientCertificate
        /// </summary>
        X509Certificate2 ClientCertificate { get; set; }
    }
}