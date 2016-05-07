namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
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
            this.OnError = new ErrorPipeline();

            this.ModulePath = modulePath;
            this.routes = new List<Route>();
        }

        /// <summary>
        /// Non-model specific data for rendering in the response
        /// </summary>
        public dynamic ViewBag
        {
            get { return this.Context == null ? null : this.Context.ViewBag; }
        }

        /// <summary>
        /// Dynamic access to text resources.
        /// </summary>
        public dynamic Text
        {
            get { return this.Context.Text; }
        }

        /// <summary>
        /// Declares a route for DELETE requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Delete(string path, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Delete<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for DELETE requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Delete<T>(string path, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Delete(path, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        /// <summary>
        /// Declares a route for DELETE requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Delete(string path, Func<dynamic, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Delete<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for DELETE requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Delete<T>(string path, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Delete(path, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        /// <summary>
        /// Declares a route for DELETE requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Delete(string path, Func<dynamic, CancellationToken, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Delete<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for DELETE requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Delete<T>(string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddRoute("DELETE", path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for GET requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Get(string path, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Get<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for GET requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Get<T>(string path, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Get(path, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        /// <summary>
        /// Declares a route for GET requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Get(string path, Func<dynamic, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Get<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for GET requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Get<T>(string path, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Get(path, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        /// <summary>
        /// Declares a route for GET requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Get(string path, Func<dynamic, CancellationToken, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Get<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for GET requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Get<T>(string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddRoute("GET", path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for HEAD requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Head(string path, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Head<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for HEAD requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Head<T>(string path, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Head(path, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        /// <summary>
        /// Declares a route for HEAD requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Head(string path, Func<dynamic, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Head<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for HEAD requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Head<T>(string path, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Head(path, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        /// <summary>
        /// Declares a route for HEAD requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Head(string path, Func<dynamic, CancellationToken, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Head<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for HEAD requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Head<T>(string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddRoute("HEAD", path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for OPTIONS requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Options(string path, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Options<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for OPTIONS requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Options<T>(string path, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Options(path, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        /// <summary>
        /// Declares a route for OPTIONS requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Options(string path, Func<dynamic, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Options<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for OPTIONS requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Options<T>(string path, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Options(path, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        /// <summary>
        /// Declares a route for OPTIONS requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Options(string path, Func<dynamic, CancellationToken, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Options<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for OPTIONS requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Options<T>(string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddRoute("OPTIONS", path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for PATCH requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Patch(string path, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Patch<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for PATCH requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Patch<T>(string path, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Patch(path, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        /// <summary>
        /// Declares a route for PATCH requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Patch(string path, Func<dynamic, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Patch<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for PATCH requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Patch<T>(string path, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Patch(path, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        /// <summary>
        /// Declares a route for PATCH requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Patch(string path, Func<dynamic, CancellationToken, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Patch<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for PATCH requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Patch<T>(string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddRoute("PATCH", path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for POST requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Post(string path, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Post<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for POST requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Post<T>(string path, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Post(path, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        /// <summary>
        /// Declares a route for POST requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Post(string path, Func<dynamic, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Post<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for POST requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Post<T>(string path, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Post(path, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        /// <summary>
        /// Declares a route for POST requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Post(string path, Func<dynamic, CancellationToken, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Post<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for POST requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Post<T>(string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddRoute("POST", path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for PUT requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Put(string path, Func<dynamic, object> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Put<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for PUT requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Put<T>(string path, Func<dynamic, T> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Put(path, args => Task.FromResult(action((DynamicDictionary)args)), condition, name);
        }

        /// <summary>
        /// Declares a route for PUT requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Put(string path, Func<dynamic, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Put<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for PUT requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Put<T>(string path, Func<dynamic, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Put(path, (args, ct) => action((DynamicDictionary)args), condition, name);
        }

        /// <summary>
        /// Declares a route for PUT requests.
        /// </summary>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Put(string path, Func<dynamic, CancellationToken, Task<object>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.Put<object>(path, action, condition, name);
        }

        /// <summary>
        /// Declares a route for PUT requests.
        /// </summary>
        /// <typeparam name="T">The return type of the <paramref name="action"/></typeparam>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="name">Name of the route</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        public virtual void Put<T>(string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition = null, string name = null)
        {
            this.AddRoute("PUT", path, action, condition, name);
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
        /// Declares a route for the module
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name of the route</param>
        /// <param name="method">The HTTP method that the route will response to</param>
        /// <param name="path">The path that the route will respond to</param>
        /// <param name="action">Action that will be invoked when the route it hit</param>
        /// <param name="condition">A condition to determine if the route can be hit</param>
        protected void AddRoute<T>(string method, string path, Func<dynamic, CancellationToken, Task<T>> action, Func<NancyContext, bool> condition, string name)
        {
            this.routes.Add(new Route<T>(name ?? string.Empty, method, this.GetFullPath(path), condition, action));
        }

        private string GetFullPath(string path)
        {
            var relativePath = (path ?? string.Empty).Trim('/');
            var parentPath = (this.ModulePath ?? string.Empty).Trim('/');

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
