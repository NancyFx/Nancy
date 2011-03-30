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
        private const string Boundary = "--NancyMultiPartBoundary123124";

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserContextMultipartFormData"/> class.
        /// </summary>
        /// <param name="configuration">The configuration that should be used to create the multipart/form-data encoded data.</param>
        public BrowserContextMultipartFormData(Action<BrowserContextMultipartFormDataConfigurator> configuration)
        {
            this.Body = new MemoryStream();

            var configurator =
                new BrowserContextMultipartFormDataConfigurator(this.Body, Boundary);

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
            var builder = new StringBuilder();

            builder.Append('\r');
            builder.Append('\n');
            builder.Append(Boundary);
            builder.Append("--");

            var encodedHeaders =
                Encoding.ASCII.GetBytes(builder.ToString());

            this.Body.Write(encodedHeaders, 0, encodedHeaders.Length);
        }

        /// <summary>
        /// Provides an API for configuring a multipart/form-data encoded request body.
        /// </summary>
        public class BrowserContextMultipartFormDataConfigurator
        {
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
                this.AddFileHeaders(name, fileName, contentType);
                this.AddContent(file);
            }

            private void AddContent(Stream file)
            {
                file.Position = 0;
                file.CopyTo(this.body);
            }

            private void AddFileHeaders(string name, string filename, string contentType)
            {
                var builder = new StringBuilder();

                builder.Append('\r');
                builder.Append('\n'); 
                builder.Append(this.boundary);
                builder.Append('\r');
                builder.Append('\n');  
                builder.AppendFormat(@"Content-Disposition: form-data; name=""{0}""; filename=""{1}""", name, filename);
                builder.Append('\r');
                builder.Append('\n');
                builder.AppendFormat(@"Content-Type: {0}", contentType);
                builder.Append('\r');
                builder.Append('\n');
                builder.Append('\r');
                builder.Append('\n');

                var encodedHeaders =
                    Encoding.ASCII.GetBytes(builder.ToString());

                this.body.Write(encodedHeaders, 0, encodedHeaders.Length);
            }
        }
    }
}