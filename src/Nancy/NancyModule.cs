namespace Nancy
{
    using System;
    using System.Collections.Generic;

    public abstract class NancyModule
    {
        private readonly IDictionary<string, IDictionary<string, Func<dynamic, Response>>> moduleRoutes;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyModule"/> class.
        /// </summary>
        protected NancyModule() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyModule"/> class.
        /// </summary>
        /// <param name="modulePath">A <see cref="string"/> containing the root relative path that all paths in the module will be a subset of.</param>
        protected NancyModule(string modulePath)
        {
            this.ModulePath = modulePath;
            this.moduleRoutes = new Dictionary<string, IDictionary<string, Func<dynamic, Response>>>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets all the routes that have been declared for the request <paramref name="method"/>.
        /// </summary>
        /// <param name="method">A <see cref="string"/> containing the http request method for which the routes should be returned.</param>
        /// <returns>An <see cref="IDictionary{TKey,TValue}"/> containing the routes.</returns>
        /// <remarks>Valid values are delete, get, post and put. The parameter is not case sensitive.</remarks>
        public IDictionary<string, Func<dynamic, Response>> GetRoutes(string method)
        {
            if (method.Equals("HEAD", StringComparison.OrdinalIgnoreCase))
            {
                method = "GET";
            }

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

        /// <summary>
        /// Gets or sets an <see cref="IRequest"/> instance that represents the current request.
        /// </summary>
        /// <value>An <see cref="IRequest"/> instance.</value>
        public IRequest Request { get; set; }

        /// <summary>
        /// An extension point for adding support for view engines.
        /// </summary>
        /// <value>This property will always return <see langword="null" /> because it acts as an extension point.</value>
        /// <remarks>Extension methods to this property should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
        public IViewEngine View { get; private set; }

        /// <summary>
        /// An extension point for adding support for formatting response contents.
        /// </summary>
        /// <value>This property will always return <see langword="null" /> because it acts as an extension point.</value>
        /// <remarks>Extension methods to this property should always return <see cref="Response"/> or one of the types that can implicitly be types into a <see cref="Response"/>.</remarks>
        public IResponseFormatter Response { get; private set; }
    }
}