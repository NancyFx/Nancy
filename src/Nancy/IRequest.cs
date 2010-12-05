namespace Nancy
{
    using System.Collections.Generic;
    using System.IO;

    public interface IRequest
    {
        string Uri { get; }

        string Method { get; }

        IDictionary<string, IEnumerable<string>> Headers { get; }

        string ContentType { get; }

        int ContentLenght { get; }

        Stream Body { get; }
    }

    public class Request : IRequest
    {
        public Request(string method, string uri)
        {
            this.Uri = uri;
            this.Method = method;
        }

        public string Uri { get; private set; }

        public string Method { get; private set; }

        public IDictionary<string, IEnumerable<string>> Headers { get; private set; }

        public string ContentType { get; private set; }

        public int ContentLenght { get; private set; }

        public  Stream Body { get; private set; }
    }
}