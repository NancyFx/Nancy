namespace Nancy.Testing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a way to define a Nancy module though an API.
    /// </summary>
    public class ConfigurableNancyModule : NancyModule
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
            private readonly ConfigurableNancyModule wrappedModule;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurableNancyModuleConfigurator"/> class.
            /// </summary>
            /// <param name="module">The <see cref="ConfigurableNancyModule"/> that should be configured.</param>
            public ConfigurableNancyModuleConfigurator(ConfigurableNancyModule module)
            {
                this.wrappedModule = module;
            }

            /// <summary>
            /// Adds an after-request process pipeline to the wrappedModule.
            /// </summary>
            /// <param name="after">An <see cref="AfterPipeline"/> instance.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator After(AfterPipeline after)
            {
                this.wrappedModule.After = after;

                return this;
            }

            /// <summary>
            /// Adds a before-request process pipeline to the wrappedModule.
            /// </summary>
            /// <param name="before">An <see cref="BeforePipeline"/> instance.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Before(BeforePipeline before)
            {
                this.wrappedModule.Before = before;

                return this;
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Delete(string path, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Delete<object>(path, (args, module) => HttpStatusCode.OK, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Delete(string path, Func<dynamic, NancyModule, object> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Delete<object>(path, (args, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Delete<T>(string path, Func<dynamic, NancyModule, T> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Delete(path, (args, module) => Task.FromResult(action((DynamicDictionary)args, module)), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Delete(string path, Func<dynamic, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Delete<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Delete<T>(string path, Func<dynamic, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Delete(path, (args, ct, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Delete(string path, Func<dynamic, CancellationToken, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Delete<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for DELETE requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Delete<T>(string path, Func<dynamic, CancellationToken, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                this.wrappedModule.Delete(path, (args, ct) => action((DynamicDictionary)args, ct, this.wrappedModule), condition ?? (ctx => true), name ?? string.Empty);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Get(string path, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Get<object>(path, (args, module) => HttpStatusCode.OK, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            public ConfigurableNancyModuleConfigurator Get(string path, Func<dynamic, NancyModule, object> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Get<object>(path, (args, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Get<T>(string path, Func<dynamic, NancyModule, T> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Get(path, (args, module) => Task.FromResult(action((DynamicDictionary)args, module)), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Get(string path, Func<dynamic, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Get<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Get<T>(string path, Func<dynamic, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Get(path, (args, ct, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Get(string path, Func<dynamic, CancellationToken, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Get<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for GET requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Get<T>(string path, Func<dynamic, CancellationToken, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                this.wrappedModule.Get(path, (args, ct) => action((DynamicDictionary)args, ct, this.wrappedModule), condition ?? (ctx => true), name ?? string.Empty);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for HEAD requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Head(string path, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Head<object>(path, (args, module) => HttpStatusCode.OK, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for HEAD requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            public ConfigurableNancyModuleConfigurator Head(string path, Func<dynamic, NancyModule, object> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Head<object>(path, (args, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for HEAD requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Head<T>(string path, Func<dynamic, NancyModule, T> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Head(path, (args, module) => Task.FromResult(action((DynamicDictionary)args, module)), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for HEAD requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Head(string path, Func<dynamic, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Head<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for HEAD requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Head<T>(string path, Func<dynamic, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Head(path, (args, ct, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for HEAD requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Head(string path, Func<dynamic, CancellationToken, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Head<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for HEAD requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Head<T>(string path, Func<dynamic, CancellationToken, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                this.wrappedModule.Head(path, (args, ct) => action((DynamicDictionary)args, ct, this.wrappedModule), condition ?? (ctx => true), name ?? string.Empty);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Options(string path, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Options<object>(path, (args, module) => HttpStatusCode.OK, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            public ConfigurableNancyModuleConfigurator Options(string path, Func<dynamic, NancyModule, object> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Options<object>(path, (args, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Options<T>(string path, Func<dynamic, NancyModule, T> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Options(path, (args, module) => Task.FromResult(action((DynamicDictionary)args, module)), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Options(string path, Func<dynamic, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Options<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Options<T>(string path, Func<dynamic, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Options(path, (args, ct, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Options(string path, Func<dynamic, CancellationToken, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Options<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for OPTIONS requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Options<T>(string path, Func<dynamic, CancellationToken, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                this.wrappedModule.Options(path, (args, ct) => action((DynamicDictionary)args, ct, this.wrappedModule), condition ?? (ctx => true), name ?? string.Empty);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Patch(string path, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Patch<object>(path, (args, module) => HttpStatusCode.OK, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            public ConfigurableNancyModuleConfigurator Patch(string path, Func<dynamic, NancyModule, object> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Patch<object>(path, (args, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Patch<T>(string path, Func<dynamic, NancyModule, T> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Patch(path, (args, module) => Task.FromResult(action((DynamicDictionary)args, module)), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Patch(string path, Func<dynamic, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Patch<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Patch<T>(string path, Func<dynamic, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Patch(path, (args, ct, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Patch(string path, Func<dynamic, CancellationToken, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Patch<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PATCH requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Patch<T>(string path, Func<dynamic, CancellationToken, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                this.wrappedModule.Patch(path, (args, ct) => action((DynamicDictionary)args, ct, this.wrappedModule), condition ?? (ctx => true), name ?? string.Empty);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Post(string path, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Post<object>(path, (args, module) => HttpStatusCode.OK, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            public ConfigurableNancyModuleConfigurator Post(string path, Func<dynamic, NancyModule, object> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Post<object>(path, (args, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Post<T>(string path, Func<dynamic, NancyModule, T> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Post(path, (args, module) => Task.FromResult(action((DynamicDictionary)args, module)), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Post(string path, Func<dynamic, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Post<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Post<T>(string path, Func<dynamic, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Post(path, (args, ct, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Post(string path, Func<dynamic, CancellationToken, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Post<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for POST requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Post<T>(string path, Func<dynamic, CancellationToken, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                this.wrappedModule.Post(path, (args, ct) => action((DynamicDictionary)args, ct, this.wrappedModule), condition ?? (ctx => true), name ?? string.Empty);
                return this;
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            /// <remarks>This will add a route with a condition that is always evaluates to <see langword="true"/> and an action that returns <see cref="HttpStatusCode.OK"/>.</remarks>
            public ConfigurableNancyModuleConfigurator Put(string path, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Put<object>(path, (args, module) => HttpStatusCode.OK, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            public ConfigurableNancyModuleConfigurator Put(string path, Func<dynamic, NancyModule, object> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Put<object>(path, (args, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Put<T>(string path, Func<dynamic, NancyModule, T> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Put(path, (args, module) => Task.FromResult(action((DynamicDictionary)args, module)), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Put(string path, Func<dynamic, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Put<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Put<T>(string path, Func<dynamic, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Put(path, (args, ct, module) => action((DynamicDictionary)args, module), condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Put(string path, Func<dynamic, CancellationToken, NancyModule, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                return this.Put<object>(path, action, condition, name);
            }

            /// <summary>
            /// Adds a route that is valid for PUT requests.
            /// </summary>
            /// <typeparam name="T">The return type of the route.</typeparam>
            /// <param name="path">The path that the route should be registered for.</param>
            /// <param name="action">Action that will be invoked when the route it hit</param>
            /// <param name="condition">A condition to determine if the route can be hit.</param>
            /// <param name="name">Name of the route.</param>
            /// <returns>An instance to the current <see cref="ConfigurableNancyModuleConfigurator"/>.</returns>
            public ConfigurableNancyModuleConfigurator Put<T>(string path, Func<dynamic, CancellationToken, NancyModule, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
            {
                this.wrappedModule.Put(path, (args, ct) => action((DynamicDictionary)args, ct, this.wrappedModule), condition ?? (ctx => true), name ?? string.Empty);
                return this;
            }
        }
    }
}