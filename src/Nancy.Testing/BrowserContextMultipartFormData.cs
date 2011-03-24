namespace Nancy.Tests
{
    using System;
    using System.IO;
    using System.Text;

    public class BrowserContextMultipartFormData
    {
        private readonly string Boundary = "--NancyMultiPartBoundary123124";

        public BrowserContextMultipartFormData(Action<BrowserContextMultipartFormDataConfigurator> closure)
        {
            this.Body = new MemoryStream();

            var configurator =
                new BrowserContextMultipartFormDataConfigurator(this.Body, this.Boundary);

            closure.Invoke(configurator);
            this.TerminateBoundary();
            this.Body.Position = 0;

            var content =
                (new StreamReader(this.Body)).ReadToEnd();

            this.Body.Position = 0;
            var ffg = 10;
        }

        public Stream Body { get; private set; }

        private void InitializeBoundary()
        {
            // 
            var builder = new StringBuilder();

            builder.Append('\r');
            builder.Append('\n');
            builder.AppendFormat(@"Content-Type: multipart/form-data; boundary={0}", this.Boundary.Substring(6));
            builder.Append('\r');
            builder.Append('\n');
            builder.Append('\r');
            builder.Append('\n'); 

            var encodedHeaders =
                Encoding.ASCII.GetBytes(builder.ToString());

            this.Body.Write(encodedHeaders, 0, encodedHeaders.Length);
        }

        private void TerminateBoundary()
        {
            var builder = new StringBuilder();

            builder.Append('\r');
            builder.Append('\n');
            builder.Append(this.Boundary);
            builder.Append("--");

            var encodedHeaders =
                Encoding.ASCII.GetBytes(builder.ToString());

            this.Body.Write(encodedHeaders, 0, encodedHeaders.Length);
        }

        public class BrowserContextMultipartFormDataConfigurator
        {
            private readonly Stream body;
            private readonly string boundary;

            public BrowserContextMultipartFormDataConfigurator(Stream body, string boundary)
            {
                this.body = body;
                this.boundary = boundary;
            }

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