namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    [InheritedExport]
    public interface INancy
    {
        IDictionary<string, Func<dynamic, Response>> Get { get; }

        IDictionary<string, Func<dynamic, Response>> Post { get; }
    }

    public class NancyModule : INancy
    {
        public NancyModule()
        {
            this.Get = new Dictionary<string, Func<dynamic, Response>>();
            this.Post = new Dictionary<string, Func<dynamic, Response>>();
        }

        public IDictionary<string, Func<dynamic, Response>> Get { get; private set; }

        public IDictionary<string, Func<dynamic, Response>> Post { get; private set; }
    }
}