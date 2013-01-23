namespace Nancy.HttpParser
{
    internal class RequestLine
    {
        private readonly string method;
        private readonly string uri;
        private readonly string httpVersion;

        public RequestLine(string method, string uri, string httpVersion)
        {
            this.method = method;
            this.uri = uri;
            this.httpVersion = httpVersion;
        }

        public string HttpVersion
        {
            get { return httpVersion; }
        }

        public string Uri
        {
            get { return uri; }
        }

        public string Method
        {
            get { return method; }
        }
    }
}