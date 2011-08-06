namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using ModelBinding;
    using Nancy.Routing;
    using Nancy.Session;
    using Nancy.ViewEngines;
    using Nancy.Extensions;

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
            this.After = new AfterPipeline();
            this.Before = new BeforePipeline();
            this.ModulePath = modulePath;
            this.routes = new List<Route>();
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
        /// Gets or sets the current Nancy context
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        public NancyContext Context { get; set; }

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
        /// Gets or sets an <see cref="Request"/> instance that represents the current request.
        /// </summary>
        /// <value>An <see cref="Request"/> instance.</value>
        public Request Request
        {
            get { return this.Context.Request; }
            set { this.Context.Request = value; }
        }

        /// <summary>
        /// Gets all declared routes by the module.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> instance, containing all <see cref="Route"/> instances declared by the module.</value>
        public IEnumerable<Route> Routes
        {
            get { return this.routes.AsReadOnly(); }
        }

        /// <summary>
        /// An extension point for adding support for formatting response contents.
        /// </summary>
        /// <value>This property will always return <see langword="null" /> because it acts as an extension point.</value>
        /// <remarks>Extension methods to this property should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
        public IResponseFormatter Response { get; set; }

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

        /// <summary>
        /// The extension point for accessing the view engines in Nancy.
        /// </summary>
        /// <value>An <see cref="IViewFactory"/> instance.</value>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IViewFactory ViewFactory { get; set; }

        /// <summary>
        /// Gets or sets the model binder locator
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IModelBinderLocator ModelBinderLocator { get; set; }

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
            public Func<dynamic, Response> this[string path]
            {
                set { this.AddRoute(path, null, value); }
            }

            /// <summary>
            /// Defines a Nancy route for the specified <paramref name="path"/> and <paramref name="condition"/>.
            /// </summary>
            /// <value>A delegate that is used to invoke the route.</value>
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

        /// <summary>
        /// Helper class for rendering a view from a route handler.
        /// </summary>
        public class ViewRenderer : IHideObjectMembers
        {
            private readonly NancyModule module;

            /// <summary>
            /// Initializes a new instance of the <see cref="ViewRenderer"/> class.
            /// </summary>
            /// <param name="module">The <see cref="NancyModule"/> instance that is rendering the view.</param>
            public ViewRenderer(NancyModule module)
            {
                this.module = module;
            }

            /// <summary>
            /// Renders the view with its name resolved from the model type, and model defined by the <paramref name="model"/> parameter.
            /// </summary>
            /// <param name="model">The model that should be passed into the view.</param>
            /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
            /// <remarks>The view name is model.GetType().Name with any Model suffix removed.</remarks>
            public Response this[dynamic model]
            {
                get { return this.module.ViewFactory.RenderView(null, model, this.GetViewLocationContext()); }
            }

            /// <summary>
            /// Renders the view with the name defined by the <paramref name="viewName"/> parameter.
            /// </summary>
            /// <param name="viewName">The name of the view to render.</param>
            /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
            /// <remarks>The extension in the view name is optional. If it is omitted, then Nancy will try to resolve which of the available engines that should be used to render the view.</remarks>
            public Response this[string viewName]
            {
                get { return this.module.ViewFactory.RenderView(viewName, null, this.GetViewLocationContext()); }
            }

            /// <summary>
            /// Renders the view with the name and model defined by the <paramref name="viewName"/> and <paramref name="model"/> parameters.
            /// </summary>
            /// <param name="viewName">The name of the view to render.</param>
            /// <param name="model">The model that should be passed into the view.</param>
            /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
            /// <remarks>The extension in the view name is optional. If it is omitted, then Nancy will try to resolve which of the available engines that should be used to render the view.</remarks>
            public Response this[string viewName, dynamic model]
            {
                get { return this.module.ViewFactory.RenderView(viewName, model, this.GetViewLocationContext()); }
            }

            private ViewLocationContext GetViewLocationContext()
            {
                return new ViewLocationContext
                       {
                           ModulePath = module.ModulePath,
                           ModuleName = module.GetModuleName(),
                           Context = module.Context
                       };
            }
        }
    }
}
