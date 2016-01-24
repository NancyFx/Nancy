namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Nancy.Configuration;
    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Session;
    using Nancy.Validation;
    using Nancy.ViewEngines;

    /// <summary>
    /// Basic class containing the functionality for defining routes and actions in Nancy.
    /// </summary>
    public abstract class NancyModule : INancyModule, IHideObjectMembers
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

        public dynamic Text
        {
            get { return this.Context.Text; }
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
        public RouteBuilder Get
        {
            get { return new RouteBuilder("GET", this); }
        }

        /// <summary>
        /// Gets <see cref="RouteBuilder"/> for declaring actions for HEAD requests.
        /// </summary>
        /// <value>A <see cref="RouteBuilder"/> instance.</value>
        public RouteBuilder Head
        {
            get { return new RouteBuilder("HEAD", this); }
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
        /// Get the root path of the routes in the current module.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> containing the root path of the module or <see langword="null" />
        /// if no root path should be used.</value><remarks>All routes will be relative to this root path.
        /// </remarks>
        public string ModulePath { get; protected set; }

        /// <summary>
        /// Gets all declared routes by the module.
        /// </summary>
        /// <value>A <see cref="IEnumerable{T}"/> instance, containing all <see cref="Route"/> instances declared by the module.</value>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual IEnumerable<Route> Routes
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
        /// <value>A <see cref="ViewRenderer"/> instance that is used to determine which view that should be rendered.</value>
        public ViewRenderer View
        {
            get { return new ViewRenderer(this); }
        }

        /// <summary>
        /// Used to negotiate the content returned based on Accepts header.
        /// </summary>
        /// <value>A <see cref="Negotiator"/> instance that is used to negotiate the content returned.</value>
        public Negotiator Negotiate
        {
            get { return new Negotiator(this.Context); }
        }

        /// <summary>
        /// Gets or sets the validator locator.
        /// </summary>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IModelValidatorLocator ValidatorLocator { get; set; }

        /// <summary>
        /// Gets or sets an <see cref="Request"/> instance that represents the current request.
        /// </summary>
        /// <value>An <see cref="Request"/> instance.</value>
        public virtual Request Request
        {
            get { return this.Context.Request; }
            set { this.Context.Request = value; }
        }

        /// <summary>
        /// The extension point for accessing the view engines in Nancy.
        /// </summary><value>An <see cref="IViewFactory" /> instance.</value>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IViewFactory ViewFactory { get; set; }

        /// <summary><para>
        /// The post-request hook
        /// </para><para>
        /// The post-request hook is called after the response is created by the route execution.
        /// It can be used to rewrite the response or add/remove items from the context.
        /// </para>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        /// </summary>
        public AfterPipeline After { get; set; }

        /// <summary>
        /// <para>
        /// The pre-request hook
        /// </para>
        /// <para>
        /// The PreRequest hook is called prior to executing a route. If any item in the
        /// pre-request pipeline returns a response then the route is not executed and the
        /// response is returned.
        /// </para>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        /// </summary>
        public BeforePipeline Before { get; set; }

        /// <summary>
        /// <para>
        /// The error hook
        /// </para>
        /// <para>
        /// The error hook is called if an exception is thrown at any time during executing
        /// the PreRequest hook, a route and the PostRequest hook. It can be used to set
        /// the response and/or finish any ongoing tasks (close database session, etc).
        /// </para>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        /// </summary>
        public ErrorPipeline OnError { get; set; }

        /// <summary>
        /// Gets or sets the current Nancy context
        /// </summary>
        /// <value>A <see cref="NancyContext" /> instance.</value>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        public NancyContext Context { get; set; }

        /// <summary>
        /// An extension point for adding support for formatting response contents.
        /// </summary><value>This property will always return <see langword="null" /> because it acts as an extension point.</value><remarks>Extension methods to this property should always return <see cref="P:Nancy.NancyModuleBase.Response" /> or one of the types that can implicitly be types into a <see cref="P:Nancy.NancyModuleBase.Response" />.</remarks>
        public IResponseFormatter Response { get; set; }

        /// <summary>
        /// Gets or sets the model binder locator
        /// </summary>
        /// <remarks>This is automatically set by Nancy at runtime.</remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IModelBinderLocator ModelBinderLocator { get; set; }

        /// <summary>
        /// Gets or sets the model validation result
        /// </summary>
        /// <remarks>This is automatically set by Nancy at runtime when you run validation.</remarks>
        public virtual ModelValidationResult ModelValidationResult
        {
            get { return this.Context == null ? null : this.Context.ModelValidationResult; }
            set
            {
                if (this.Context != null)
                {
                    this.Context.ModelValidationResult = value;
                }
            }
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
            /// <param name="parentModule">The <see cref="INancyModule"/> that the route is being configured for.</param>
            public RouteBuilder(string method, NancyModule parentModule)
            {
                this.method = method;
                this.parentModule = parentModule;
            }

            /// <summary>
            /// Defines an async route for the specified <paramref name="path"/>
            /// </summary>
            public Func<dynamic, CancellationToken, Task<dynamic>> this[string path]
            {
                set { this.AddRoute(string.Empty, path, null, value); }
            }

            /// <summary>
            /// Defines an async route for the specified <paramref name="path"/> and <paramref name="condition"/>.
            /// </summary>
            public Func<dynamic, CancellationToken, Task<dynamic>> this[string path, Func<NancyContext, bool> condition]
            {
                set { this.AddRoute(string.Empty, path, condition, value); }
            }

            /// <summary>
            /// Defines an async route for the specified <paramref name="path"/> and <paramref name="name"/>
            /// </summary>
            public Func<dynamic, CancellationToken, Task<dynamic>> this[string name, string path]
            {
                set { this.AddRoute(name, path, null, value); }
            }

            /// <summary>
            /// Defines an async route for the specified <paramref name="path"/>, <paramref name="condition"/> and <paramref name="name"/>
            /// </summary>
            public Func<dynamic, CancellationToken, Task<dynamic>> this[string name, string path, Func<NancyContext, bool> condition]
            {
                set { this.AddRoute(name, path, condition, value); }
            }

            protected void AddRoute(string name, string path, Func<NancyContext, bool> condition, Func<dynamic, CancellationToken, Task<dynamic>> value)
            {
                var fullPath = GetFullPath(path);

                this.parentModule.routes.Add(new Route(name, this.method, fullPath, condition, value));
            }

            private string GetFullPath(string path)
            {
                var relativePath = (path ?? string.Empty).Trim('/');
                var parentPath = (this.parentModule.ModulePath ?? string.Empty).Trim('/');

                if (string.IsNullOrEmpty(parentPath))
                {
                    return string.Concat("/", relativePath);
                }

                if (string.IsNullOrEmpty(relativePath))
                {
                    return string.Concat("/", parentPath);
                }

                return string.Concat("/", parentPath, "/", relativePath);
            }
        }
    }
}
