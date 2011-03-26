namespace Nancy.Testing.Fakes
{
    using System;

    /// <summary>
    /// Provides a way to define a Nancy module though an API.
    /// </summary>
    public class FakeNancyModule : NancyModule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeNancyModule"/> class.
        /// </summary>
        public FakeNancyModule()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeNancyModule"/> class.
        /// </summary>
        /// <param name="closure">The configuration of the module.</param>
        public FakeNancyModule(Action<FakeNancyModuleConfigurator> closure)
        {
            var configurator =
                new FakeNancyModuleConfigurator(this);

            closure.Invoke(configurator);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeNancyModule"/> class.
        /// </summary>
        /// <param name="modulePath">The path that all routes in the module should be relative too.</param>
        /// <param name="closure">The configuration of the module.</param>
        public FakeNancyModule(string modulePath, Action<FakeNancyModuleConfigurator> closure)
            : base(modulePath)
        {
            var configurator =
                new FakeNancyModuleConfigurator(this);

            closure.Invoke(configurator);
        }

        public class FakeNancyModuleConfigurator : IHideObjectMembers
        {
            private readonly FakeNancyModule module;

            public FakeNancyModuleConfigurator(FakeNancyModule module)
            {
                this.module = module;
            }

            /// <summary>
            /// Adds an after-request process pipeline to the module.
            /// </summary>
            /// <param name="after">An <see cref="AfterPipeline"/> instance.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            public FakeNancyModuleConfigurator After(AfterPipeline after)
            {
                this.module.After = after;

                return this;
            }

            /// <summary>
            /// Adds a before-request process pipeline to the module.
            /// </summary>
            /// <param name="before">An <see cref="BeforePipeline"/> instance.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            public FakeNancyModuleConfigurator Before(BeforePipeline before)
            {
                this.module.Before = before;

                return this;
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public FakeNancyModuleConfigurator Delete(string path)
            {
                return this.Delete(path, condition => true, action => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public FakeNancyModuleConfigurator Delete(string path, Func<dynamic, Response> action)
            {
                return this.Delete(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be furfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            public FakeNancyModuleConfigurator Delete(string path, Func<NancyContext, bool> condition, Func<dynamic, Response> action)
            {
                this.module.Delete[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public FakeNancyModuleConfigurator Get(string path)
            {
                return this.Get(path, condition => true, action => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public FakeNancyModuleConfigurator Get(string path, Func<dynamic, Response> action)
            {
                return this.Get(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be furfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            public FakeNancyModuleConfigurator Get(string path, Func<NancyContext, bool> condition, Func<dynamic, Response> action)
            {
                this.module.Get[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public FakeNancyModuleConfigurator Post(string path)
            {
                return this.Post(path, condition => true, action => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public FakeNancyModuleConfigurator Post(string path, Func<dynamic, Response> action)
            {
                return this.Post(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be furfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            public FakeNancyModuleConfigurator Post(string path, Func<NancyContext, bool> condition, Func<dynamic, Response> action)
            {
                this.module.Post[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public FakeNancyModuleConfigurator Put(string path)
            {
                return this.Put(path, condition => true, action => HttpStatusCode.OK);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/>.</remarks>
            public FakeNancyModuleConfigurator Put(string path, Func<dynamic, Response> action)
            {
                return this.Put(path, condition => true, action);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">The condition that has to be furfilled in order for the route to be invoked</param>
            /// <param name="action">The action that should be invoked by the route.</param>
            /// <returns>An instance to the current <see cref="FakeNancyModuleConfigurator"/>.</returns>
            public FakeNancyModuleConfigurator Put(string path, Func<NancyContext, bool> condition, Func<dynamic, Response> action)
            {
                this.module.Post[path, GetSafeRouteCondition(condition)] = GetSafeRouteAction(action);
                return this;
            }

            private static Func<dynamic, Response> GetSafeRouteAction(Func<dynamic, Response> action)
            {
                return action ?? (x => HttpStatusCode.OK);
            }

            private static Func<NancyContext, bool> GetSafeRouteCondition(Func<NancyContext, bool> condition)
            {
                return condition ?? (x => true);
            }
        }
    }
}