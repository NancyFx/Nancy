namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    [InheritedExport]
    public abstract class NancyModule
    {
        private readonly IDictionary<string, IDictionary<string, Func<dynamic, Response>>> moduleRoutes;

        protected NancyModule() : this(string.Empty)
        {
        }

        protected NancyModule(string modulePath)
        {
            this.ModulePath = modulePath;
            this.moduleRoutes = new Dictionary<string, IDictionary<string, Func<dynamic, Response>>>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, Func<dynamic, Response>> GetRoutes(string method)
        {
            IDictionary<string, Func<dynamic, Response>> routes;

            if (!this.moduleRoutes.TryGetValue(method, out routes))
            {
                routes = new Dictionary<string, Func<dynamic, Response>>(StringComparer.OrdinalIgnoreCase);
                this.moduleRoutes[method] = routes;
            }

            return routes;
        }

        public IDictionary<string, Func<dynamic, Response>> Delete
        {
            get { return GetRoutes("DELETE"); }
        }

        public IDictionary<string, Func<dynamic, Response>> Get
        {
            get { return GetRoutes("GET"); }
        }

        public string ModulePath { get; private set; }

        public IDictionary<string, Func<dynamic, Response>> Post
        {
            get { return GetRoutes("POST"); }
        }

        public IDictionary<string, Func<dynamic, Response>> Put
        {
            get { return GetRoutes("PUT"); }
        }

        public IRequest Request { get; set; }

        public IViewEngine View { get; set; }

        public IResponseFormatter Response { get; set; }
    }
}