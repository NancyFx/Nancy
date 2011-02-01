namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Nancy.Routing;

    public abstract class NancyModule
    {
        public class RouteIndexer
        {
            private string method;

            private NancyModule parentModule;

            public RouteIndexer(string method, NancyModule parentModule)
            {
                this.method = method;
                this.parentModule = parentModule;
            }

            public Func<dynamic, Response> this[string path]
            {
                set
                {
                    this.AddRoute(path, null, value);
                }
            }

            public Func<dynamic, Response> this[string path, Func<Request, bool> condition]
            {
                set 
                {
                    this.AddRoute(path, condition, value);
                }
            }

            private void AddRoute(string path, Func<Request, bool> condition, Func<object, Response> value)
            {
                var fullPath = string.Concat(this.parentModule.ModulePath, path);

                this.parentModule.routes.Add(new Route(this.method, fullPath, condition, value));
            }
        }

        private readonly List<Route> routes;

        public IEnumerable<Route> Routes
        {
            get { return this.routes.AsReadOnly(); }
        }

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
        /// Gets or sets an <see cref="ITemplateEngineSelector"/> which represents the current application context
        /// </summary>
        public ITemplateEngineSelector TemplateEngineSelector { get; set; }

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

        /// <summary>
        /// Renders the view based on the extension without a model.
        /// </summary>
        /// <param name="name">The path to the view</param>        
        public Action<Stream> View(string name)
        {
            return View(name, (object) null);
        }

        /// <summary>
        /// Renders the view based on the extension with a model.
        /// </summary>
        /// <param name="name">The path to the view</param>
        /// <param name="model">The model to pass to the view</param>
        public Action<Stream> View<TModel>(string name, TModel model)
        {
            var extension = Path.GetExtension(name);
            var processor = TemplateEngineSelector.GetTemplateProcessor(extension) ?? TemplateEngineSelector.DefaultProcessor;
            return stream =>
                       {
                           var result = processor.RenderView(name, model);
                           result.Execute(stream);
                       };
        }
    }
}
