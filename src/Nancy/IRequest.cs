namespace Nancy
{
    using System;

    public interface IRequest
    {
        string Path { get; }

        string Verb { get; }
    }

    public class Request : IRequest
    {
        public Request(string verb, string path)
        {
            this.Path = path;
            this.Verb = verb;
        }

        public string Path { get; private set; }

        public string Verb { get; private set; }
    }
}