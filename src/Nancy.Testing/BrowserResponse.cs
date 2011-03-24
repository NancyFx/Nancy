namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// </summary>
    public class BrowserResponse
    {
        public BrowserResponse(NancyContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.Context = context;
        }

        public NancyContext Context { get; private set; }

        public HttpStatusCode StatusCode
        {
            get { return this.Context.Response.StatusCode; }
        }

        public IDictionary<string, string> Headers
        {
            get { return this.Context.Response.Headers; }
        }

        private DocumentWrapper responseBody;
        public DocumentWrapper Body
        {
            get
            {
                if (this.responseBody == null)
                {
                    using (var contentsStream = new MemoryStream())
                    {
                        this.Context.Response.Contents.Invoke(contentsStream);
                        contentsStream.Position = 0;
                        this.responseBody = new DocumentWrapper(contentsStream);
                    }
                }

                return this.responseBody;
            }
        }
    }
}