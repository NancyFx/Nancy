namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    [InheritedExport]
    public abstract class NancyModule
    {
        protected NancyModule() : this(string.Empty)
        {
        }

        protected NancyModule(string baseRoute)
        {
            this.BaseRoute = baseRoute;
            this.Delete = new Dictionary<string, Func<dynamic, Response>>();
            this.Get = new Dictionary<string, Func<dynamic, Response>>();
            this.Post = new Dictionary<string, Func<dynamic, Response>>();
            this.Put = new Dictionary<string, Func<dynamic, Response>>();
        }

        public string BaseRoute { get; private set; }

        public IDictionary<string, Func<dynamic, Response>> Delete { get; private set; }

        public IDictionary<string, Func<dynamic, Response>> Get { get; private set; }

        public IDictionary<string, Func<dynamic, Response>> Post { get; private set; }

        public IDictionary<string, Func<dynamic, Response>> Put { get; private set; }
    }
}