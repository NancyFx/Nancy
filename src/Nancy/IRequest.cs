namespace Nancy
{
    using System;

    public interface IRequest
    {
        string Route { get; }

        string Verb { get; }
    }

    public class Request : IRequest
    {
        public Request(string verb, string route)
        {
            this.Route = route;
            this.Verb = verb;
        }

        public string Route { get; private set; }

        public string Verb { get; private set; }
    }
}