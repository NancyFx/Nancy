namespace Nancy.Testing
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Contains the functionality for setting up a multipart/form-data encoded request body that should be used by an <see cref="Browser"/> instance.
    /// </summary>
    public class BrowserContextMultipartFormData
    {
        public const string DefaultBoundaryName = "--NancyMultiPartBoundary123124";

        private readonly string boundaryName;
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserContextMultipartFormData"/> class using the default boundary name
        /// </summary>
        /// <param name="configuration">The configuration that should be used to create the multipart/form-data encoded data.</param>
        public BrowserContextMultipartFormData(Action<BrowserContextMultipartFormDataConfigurator> configuration)
            : this(configuration, DefaultBoundaryName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserContextMultipartFormData"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that should be used to create the multipart/form-data encoded data.</param>
        /// <param name="boundaryName">Boundary name to be used</param>
        public BrowserContextMultipartFormData(Action<BrowserContextMultipartFormDataConfigurator> configuration, string boundaryName)
        {
            this.boundaryName = boundaryName;
            this.Body = new MemoryStream();

            var configurator =
                new BrowserContextMultipartFormDataConfigurator(this.Body, boundaryName);

            configuration.Invoke(configurator);
            this.TerminateBoundary();
            this.Body.Position = 0;

            this.Body.Position = 0;
        }

        /// <summary>
        /// Gets the <see cref="Stream"/> that should be used by the HTTP request to pass in the multipart/form-data encoded values.
        /// </summary>
        /// <value>A <see cref="Stream"/> that contains the multipart/form-data encoded values.</value>
        public Stream Body { get; private set; }

        private void TerminateBoundary()
        {
            var endBoundary = String.Format("--{0}--\r\n", this.boundaryName);

            var encodedHeaders =
                Encoding.ASCII.GetBytes(endBoundary);

            this.Body.Write(encodedHeaders, 0, encodedHeaders.Length);
        }

        /// <summary>
        /// Provides an API for configuring a multipart/form-data encoded request body.
        /// </summary>
        public class BrowserContextMultipartFormDataConfigurator
        {
            private const string CRLF = "\r\n";
            private readonly Stream body;
            private readonly string boundary;

            /// <summary>
            /// Initializes a new instance of the <see cref="BrowserContextMultipartFormDataConfigurator"/> class.
            /// </summary>
            /// <param name="body">The <see cref="Stream"/> that the values should be written to.</param>
            /// <param name="boundary">The multipart/form-data boundary that should be used in the request body.</param>
            public BrowserContextMultipartFormDataConfigurator(Stream body, string boundary)
            {
                this.body = body;
                this.boundary = boundary;
            }

            /// <summary>
            /// Adds a file to the request body.
            /// </summary>
            /// <param name="name">The name of the file http element that was used to upload the file.</param>
            /// <param name="fileName">Name of the file.</param>
            /// <param name="contentType">The mime type of file.</param>
            /// <param name="file">The content of the file</param>
            public void AddFile(string name, string fileName, string contentType, Stream file)
            {
                this.AddFieldHeaders(name, contentType, fileName);
                this.AddContent(file);
            }

            public void AddFormField(string name, string contentType, string data)
            {
                this.AddFormField(name, contentType, new MemoryStream(Encoding.ASCII.GetBytes(data)));
            }

            public void AddFormField(string name, string contentType, Stream data)
            {
                this.AddFieldHeaders(name, contentType);
                this.AddContent(data);
            }

            private void AddContent(Stream data)
            {
                data.Position = 0;
                data.CopyTo(this.body);
            }

            private void AddFieldHeaders(string name, string contentType, string filename = null)
            {
                var builder = new StringBuilder();

                builder.Append(CRLF);
                builder.Append("--" + this.boundary);
                builder.Append(CRLF);  
                builder.AppendFormat(@"Content-Disposition: form-data; name=""{0}""", name);
                if (!String.IsNullOrWhiteSpace(filename))
                {
                    builder.AppendFormat(@"; filename=""{0}""", filename);
                }
                builder.Append(CRLF);
                builder.AppendFormat(@"Content-Type: {0}", contentType);
                builder.Append(CRLF);
                builder.Append(CRLF);

                var encodedHeaders =
                    Encoding.ASCII.GetBytes(builder.ToString());

                this.body.Write(encodedHeaders, 0, encodedHeaders.Length);
            }
        }
    }
}