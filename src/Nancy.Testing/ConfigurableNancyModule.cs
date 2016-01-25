namespace Nancy.Testing
{
    using System;

    /// <summary>
    /// Provides a way to define a Nancy module though an API.
    /// </summary>
    public class ConfigurableNancyModule : LegacyNancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableNancyModule"/> class.
        /// </summary>
        public ConfigurableNancyModule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableNancyModule"/> class.
        /// </summary>
        /// <param name="closure">The configuration of the module.</param>
        public ConfigurableNancyModule(Action<ConfigurableNancyModuleConfigurator> closure)
            : this(string.Empty, closure)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableNancyModule"/> class.
        /// </summary>
        /// <param name="modulePath">The path that all routes in the module should be relative too.</param>
        /// <param name="closure">The configuration of the module.</param>
        public ConfigurableNancyModule(string modulePath, Action<ConfigurableNancyModuleConfigurator> closure)
            : base(modulePath)
        {
            var configurator =
                new ConfigurableNancyModuleConfigurator(this);

            closure.Invoke(configurator);
        }

        /// <summary>
        /// Provides an API for configuring a <see cref="ConfigurableNancyModule"/> instance.
        /// </summary>
        public class ConfigurableNancyModuleConfigurator : IHideObjectMembers
        {
            private readonly ConfigurableNancyModule module;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurableNancyModuleConfigurator"/> class.
            /// </summary>
            /// <param name="module">The <see cref="ConfigurableNancyModule"/> that should be configured.</param>
            public ConfigurableNancyModuleConfigurator(ConfigurableNancyModule module)
            {
                this.module = module;
            }

            /// <summary>
            /// Adds an after-request process pipeline to the module.
            /// </summary>
            /// <param name="after">An <see cref="AfterPipeline"/> instance.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator After(AfterPipeline after)
            {
                this.module.After = after;

                return this;
            }

            /// <summary>
            /// Adds a before-request process pipeline to the module.
            /// </summary>
            /// <param name="before">An <see cref="BeforePipeline"/> instance.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Before(BeforePipeline before)
            {
                this.module.Before = before;

                return this;
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Delete(string path)
            {
                return this.Delete(path, condition => true, (action, module) => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Delete(string path, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                return this.Delete(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be fulfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Delete(string path, Func<NancyContext, bool> condition, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                this.module.Delete[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Get(string path)
            {
                return this.Get(path, condition => true, (action, module) => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Get(string path, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                return this.Get(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be fulfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Get(string path, Func<NancyContext, bool> condition, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                this.module.Get[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Patch(string path)
            {
                return this.Patch(path, condition => true, (action, module) => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Patch(string path, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                return this.Patch(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be fulfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Patch(string path, Func<NancyContext, bool> condition, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                this.module.Patch[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Post(string path)
            {
                return this.Post(path, condition => true, (action, module) => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Post(string path, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                return this.Post(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be fulfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Post(string path, Func<NancyContext, bool> condition, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                this.module.Post[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Put(string path)
            {
                return this.Put(path, condition => true, (action, module) => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Put(string path, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                return this.Put(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be fulfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Put(string path, Func<NancyContext, bool> condition, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                this.module.Post[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Options(string path)
            {
                return this.Patch(path, condition => true, (action, module) => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Options(string path, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                return this.Options(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be fulfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Options(string path, Func<NancyContext, bool> condition, Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                this.module.Options[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            private Func<dynamic, dynamic> GetSafeRouteAction(Func<dynamic, LegacyNancyModule, dynamic> action)
            {
                if (action == null)
                {
                    return x => HttpStatusCode.OK;
                }

                return x => action.Invoke(x, this.module);
            }

            private static Func<NancyContext, bool> GetSafeRouteCondition(Func<NancyContext, bool> condition)
            {
                return condition ?? (x => true);
            }
        }
    }
}