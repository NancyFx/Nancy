namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Session;

    /// <summary>
    /// Contains the functionality for defining routes and actions in Nancy. 
    /// </summary>
    /// <value>This is the core type in the entire framework and changes to this class should not be very frequent because it represents a change to the core API of the framework.</value>
    public abstract class NancyModule : NancyModuleBase, IHideObjectMembers
    {
        private readonly List<Route> routes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyModule"/> class.
        /// </summary>
        protected NancyModule()
            : this(String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyModule"/> class.
        /// </summary>
        /// <param name="modulePath">A <see cref="string"/> containing the root relative path that all paths in the module will be a subset of.</param>
        protected NancyModule(string modulePath)
        {
            this.After = new AfterPipeline();
            this.Before = new BeforePipeline();
            this.OnError = new ErrorPipeline();
            this.ModulePath = modulePath;
            this.routes = new List<Route>();
        }

        /// <summary>
        /// Non-model specific data for rendering in the response
        /// </summary>
        public dynamic ViewBag
        {
            get
            {
                return this.Context == null ? null : this.Context.ViewBag;
            }
        }

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
        /// Gets <see cref="RouteBuilder"/> for declaring actions for OPTIONS requests.
        /// </summary>
        /// <value>A <see cref="RouteBuilder"/> instance.</value>
        public RouteBuilder Options
        {
            get { return new RouteBuilder("OPTIONS", this); }
        }

        /// <summary>
        /// Gets <see cref="RouteBuilder"/> for declaring actions for PATCH requests.
        /// </summary>
        /// <value>A <see cref="RouteBuilder"/> instance.</value>
        public RouteBuilder Patch
        {
            get { return new RouteBuilder("PATCH", this); }
        }

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
        /// Gets all declared routes by the module.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> instance, containing all <see cref="Route"/> instances declared by the module.</value>
        public override IEnumerable<Route> Routes
        {
            get { return this.routes.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public ISession Session
        {
            get { return this.Request.Session; }
        }

        /// <summary>
        /// Renders a view from inside a route handler.
        /// </summary>
        /// <value>A <see cref="ViewRenderer"/> instance that is used to determin which view that should be rendered.</value>
        public ViewRenderer View
        {
            get { return new ViewRenderer(this); }
        }

        public Negotiator Negotiate
        {
            get { return new Negotiator(this.Context); }
        }

        /// <summary>
        /// Helper class for configuring a route handler in a module.
        /// </summary>
        public class RouteBuilder : IHideObjectMembers
        {
            private readonly string method;
            private readonly NancyModule parentModule;

            /// <summary>
            /// Initializes a new instance of the <see cref="RouteBuilder"/> class.
            /// </summary>
            /// <param name="method">The HTTP request method that the route should be available for.</param>
            /// <param name="parentModule">The <see cref="NancyModule"/> that the route is being configured for.</param>
            public RouteBuilder(string method, NancyModule parentModule)
            {
                this.method = method;
                this.parentModule = parentModule;
            }

            /// <summary>
            /// Defines a Nancy route for the specified <paramref name="path"/>.
            /// </summary>
            /// <value>A delegate that is used to invoke the route.</value>
            public Func<dynamic, dynamic> this[string path]
            {
                set { this.AddRoute(path, null, value); }
            }

            /// <summary>
            /// Defines a Nancy route for the specified <paramref name="path"/> and <paramref name="condition"/>.
            /// </summary>
            /// <value>A delegate that is used to invoke the route.</value>
            public Func<dynamic, dynamic> this[string path, Func<NancyContext, bool> condition]
            {
                set { this.AddRoute(path, condition, value); }
            }

            protected void AddRoute(string path, Func<NancyContext, bool> condition, Func<dynamic, dynamic> value)
            {
                var fullPath = String.Concat(this.parentModule.ModulePath, path);

                this.parentModule.routes.Add(new Route(this.method, fullPath, condition, value));
            }
        }

        /// <summary>
        /// Helper class for rendering a view from a route handler.
        /// </summary>
        public class ViewRenderer : IHideObjectMembers
        {
            private readonly NancyModuleBase module;

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewRenderer"/> class.
            /// </summary>
            /// <param name="module">The <see cref="NancyModule"/> instance that is rendering the view.</param>
            public ViewRenderer(NancyModuleBase module)
            {
                this.module = module;
            }

            /// <summary>
            /// Renders the view with its name resolved from the model type, and model defined by the <paramref name="model"/> parameter.
            /// </summary>
            /// <param name="model">The model that should be passed into the view.</param>
            /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
            /// <remarks>The view name is model.GetType().Name with any Model suffix removed.</remarks>
            public Negotiator this[dynamic model]
            {
                get { return this.GetNegotiator(null, model); }
            }

            /// <summary>
            /// Renders the view with the name defined by the <paramref name="viewName"/> parameter.
            /// </summary>
            /// <param name="viewName">The name of the view to render.</param>
            /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
            /// <remarks>The extension in the view name is optional. If it is omitted, then Nancy will try to resolve which of the available engines that should be used to render the view.</remarks>
            public Negotiator this[string viewName]
            {
                get { return this.GetNegotiator(viewName, null); }
            }

            /// <summary>
            /// Renders the view with the name and model defined by the <paramref name="viewName"/> and <paramref name="model"/> parameters.
            /// </summary>
            /// <param name="viewName">The name of the view to render.</param>
            /// <param name="model">The model that should be passed into the view.</param>
            /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
            /// <remarks>The extension in the view name is optional. If it is omitted, then Nancy will try to resolve which of the available engines that should be used to render the view.</remarks>
            public Negotiator this[string viewName, dynamic model]
            {
                get { return this.GetNegotiator(viewName, model); }
            }

            private Negotiator GetNegotiator(string viewName, object model)
            {
                var negotiationContext = this.module.Context.NegotiationContext;

                negotiationContext.ViewName = viewName;
                negotiationContext.DefaultModel = model;
                negotiationContext.PermissableMediaRanges.Clear();
                negotiationContext.PermissableMediaRanges.Add("text/html");

                return new Negotiator(this.module.Context);
            }
        }
    }
}
