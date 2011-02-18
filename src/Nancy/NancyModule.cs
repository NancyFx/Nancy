namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using Nancy.Routing;
    using ViewEngines;

    /// <summary>
    /// Contains the functionality for defining routes and actions in Nancy. 
    /// </summary>
    /// <value>This is the core type in the entire framework and changes to this class should not be very frequent because it represents a change to the core API of the framework.</value>
    public abstract class NancyModule
    {
        private readonly List<Route> routes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyModule"/> class.
        /// </summary>
        protected NancyModule()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyModule"/> class.
        /// </summary>
        /// <param name="modulePath">A <see cref="string"/> containing the root relative path that all paths in the module will be a subset of.</param>
        protected NancyModule(string modulePath)
        {
            this.ModulePath = modulePath;
            this.routes = new List<Route>();
        }

        /// <summary>
        /// Gets all declared routes by the module.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> instance, containing all <see cref="Route"/> instances declared by the module.</value>
        public IEnumerable<Route> Routes
        {
            get { return this.routes.AsReadOnly(); }
        }

        public IViewFactory View { get; set; }

        /// <summary>
        /// Gets <see cref="RouteIndexer"/> for declaring actions for DELETE requests.
        /// </summary>
        /// <value>A <see cref="RouteIndexer"/> instance.</value>
        public RouteIndexer Delete
        {
            get { return new RouteIndexer("DELETE", this); }
        }

        /// <summary>
        /// Gets <see cref="RouteIndexer"/> for declaring actions for GET requests.
        /// </summary>
        /// <value>A <see cref="RouteIndexer"/> instance.</value>
        /// <remarks>These actions will also be used when a HEAD request is recieved.</remarks>
        public RouteIndexer Get
        {
            get { return new RouteIndexer("GET", this); }
        }

        public string ModulePath { get; private set; }

        /// <summary>
        /// Gets <see cref="RouteIndexer"/> for declaring actions for POST requests.
        /// </summary>
        /// <value>A <see cref="RouteIndexer"/> instance.</value>
        public RouteIndexer Post
        {
            get { return new RouteIndexer("POST", this); }
        }

        /// <summary>
        /// Gets <see cref="RouteIndexer"/> for declaring actions for PUT requests.
        /// </summary>
        /// <value>A <see cref="RouteIndexer"/> instance.</value>
        public RouteIndexer Put
        {
            get { return new RouteIndexer("PUT", this); }
        }

        /// <summary>
        /// Gets or sets an <see cref="Request"/> instance that represents the current request.
        /// </summary>
        /// <value>An <see cref="Request"/> instance.</value>
        public Request Request { get; set; }

        /// <summary>
        /// An extension point for adding support for formatting response contents.
        /// </summary>
        /// <value>This property will always return <see langword="null" /> because it acts as an extension point.</value>
        /// <remarks>Extension methods to this property should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
        public IResponseFormatter Response { get; private set; }

        public class RouteIndexer
        {
            private readonly string method;
            private readonly NancyModule parentModule;

            public RouteIndexer(string method, NancyModule parentModule)
            {
                this.method = method;
                this.parentModule = parentModule;
            }

            public Func<dynamic, Response> this[string path]
            {
                set { this.AddRoute(path, null, value); }
            }

            public Func<dynamic, Response> this[string path, Func<Request, bool> condition]
            {
                set { this.AddRoute(path, condition, value); }
            }

            private void AddRoute(string path, Func<Request, bool> condition, Func<object, Response> value)
            {
                var fullPath = string.Concat(this.parentModule.ModulePath, path);

                this.parentModule.routes.Add(new Route(this.method, fullPath, condition, value));
            }
        }
    }
}
