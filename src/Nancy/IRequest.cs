namespace Nancy
{
    using System;

    public interface IRequest
    {
        Uri Route { get; }

        string Verb { get; }
    }

    public class Request : IRequest
    {
        public Request(string verb, Uri route)
        {
            this.Route = route;
            this.Verb = verb;
        }

        public Uri Route { get; private set; }

        public string Verb { get; private set; }
    }
}