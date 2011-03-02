namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using Nancy.Routing;
    using Session;

    using ViewEngines;

    /// <summary>
    /// Contains the functionality for defining routes and actions in Nancy. 
    /// </summary>
    /// <value>This is the core type in the entire framework and changes to this class should not be very frequent because it represents a change to the core API of the framework.</value>
    public abstract class NancyModule : IHideObjectMembers
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
            this.Before = new BeforePipeline();
            this.After = new AfterPipeline();
        }

        /// <summary>
        /// <para>
        /// The post-request hook
        /// </para>
        /// <para>
        /// The post-request hook is called after the response is created by the route execution.
        /// It can be used to rewrite the response or add/remove items from the context.
        /// </para>
        /// </summary>
        public AfterPipeline After { get; protected set; }

        /// <summary>
        /// <para>
        /// The pre-request hook
        /// </para>
        /// <para>
        /// The PreRequest hook is called prior to executing a route. If any item in the
        /// pre-request pipeline returns a response then the route is not executed and the
        /// response is returned.
        /// </para>
        /// </summary>
        public BeforePipeline Before { get; protected set; }

        /// <summary>
        /// Gets all declared routes by the module.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> instance, containing all <see cref="Route"/> instances declared by the module.</value>
        public IEnumerable<Route> Routes
        {
            get { return this.routes.AsReadOnly(); }
        }

        /// <summary>
        /// The extension point for accessing the view engines in Nancy.
        /// </summary>
        /// <value>An <see cref="IViewFactory"/> instance.</value>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        public IViewFactory View { get; set; }

        /// <summary>
        /// Gets <see cref="RouteBuilder"/> for declaring actions for DELETE requests.
        /// </summary>
        /// <value>A <see cref="RouteBuilder"/> instance.</value>
        public RouteBuilder Delete
        {
            get { return new RouteBuilder("DELETE", this); }
        }

        /// <summary>
        /// Gets <see cref="RouteBuilder"/> for declaring actions for GET requests.
        /// </summary>
        /// <value>A <see cref="RouteBuilder"/> instance.</value>
        /// <remarks>These actions will also be used when a HEAD request is recieved.</remarks>
        public RouteBuilder Get
        {
            get { return new RouteBuilder("GET", this); }
        }

        /// <summary>
        /// Get the root path of the routes in the current module.
        /// </summary>
        /// <value>A <see cref="string"/> containing the root path of the module or <see langword="null"/> if no root path should be used.</value>
        /// <remarks>All routes will be relative to this root path.</remarks>
        public string ModulePath { get; private set; }

        /// <summary>
        /// Gets <see cref="RouteBuilder"/> for declaring actions for POST requests.
        /// </summary>
        /// <value>A <see cref="RouteBuilder"/> instance.</value>
        public RouteBuilder Post
        {
            get { return new RouteBuilder("POST", this); }
        }

        /// <summary>
        /// Gets <see cref="RouteBuilder"/> for declaring actions for PUT requests.
        /// </summary>
        /// <value>A <see cref="RouteBuilder"/> instance.</value>
        public RouteBuilder Put
        {
            get { return new RouteBuilder("PUT", this); }
        }

        /// <summary>
        /// Gets or sets an <see cref="Request"/> instance that represents the current request.
        /// </summary>
        /// <value>An <see cref="Request"/> instance.</value>
        public Request Request
        {
            get { return this.Context.Request; }
            set { this.Context.Request = value; }
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public ISession Session
        {
            get
            {
                return this.Request.Session;
            }
        }

        /// <summary>
        /// An extension point for adding support for formatting response contents.
        /// </summary>
        /// <value>This property will always return <see langword="null" /> because it acts as an extension point.</value>
        /// <remarks>Extension methods to this property should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
        public IResponseFormatter Response { get; set; }

        /// <summary>
        /// Gets or sets the current Nancy context
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        public NancyContext Context { get; set; }

        public class RouteBuilder : IHideObjectMembers
        {
            private readonly string method;
            private readonly NancyModule parentModule;

            public RouteBuilder(string method, NancyModule parentModule)
            {
                this.method = method;
                this.parentModule = parentModule;
            }

            public Func<dynamic, Response> this[string path]
            {
                set { this.AddRoute(path, null, value); }
            }

            public Func<dynamic, Response> this[string path, Func<NancyContext, bool> condition]
            {
                set { this.AddRoute(path, condition, value); }
            }

            private void AddRoute(string path, Func<NancyContext, bool> condition, Func<object, Response> value)
            {
                var fullPath = string.Concat(this.parentModule.ModulePath, path);

                this.parentModule.routes.Add(new Route(this.method, fullPath, condition, value));
            }
        }
    }
}
