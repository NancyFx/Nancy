namespace Nancy.Demo.CustomModule
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Nancy.ModelBinding;
    using Nancy.Responses.Negotiation;
    using Nancy.Routing;
    using Nancy.Validation;
    using Nancy.ViewEngines;

    /// <summary>
    /// A custom INancyModule implementation that uses
    /// attributes on methods (eugh!) to define routes.
    /// Nobody in their right mind would write a web framework
    /// that uses attributes for routing ;-)
    /// </summary>
    public abstract class UglifiedNancyModule : INancyModule
    {
        public AfterPipeline After { get; set; }

        public BeforePipeline Before { get; set; }

        public ErrorPipeline OnError { get; set; }

        public NancyContext Context { get; set; }

        public IResponseFormatter Response { get; set; }

        public IModelBinderLocator ModelBinderLocator { get; set; }

        public ModelValidationResult ModelValidationResult { get; set; }

        public IModelValidatorLocator ValidatorLocator { get; set; }

        public Request Request { get; set; }

        public IViewFactory ViewFactory { get; set; }

        public string ModulePath { get; private set; }

        public ViewRenderer View
        {
            get { return new ViewRenderer(this); }
        }

        public Negotiator Negotiate
        {
            get { return new Negotiator(this.Context); }
        }

        public UglifiedNancyModule()
            : this(string.Empty)
        {
        }

        public IEnumerable<Route> Routes
        {
            get
            {
                return this.GetRoutes();
            }
        }

        public dynamic Text { get; set; }

        private UglifiedNancyModule(string modulePath)
        {
            this.After = new AfterPipeline();
            this.Before = new BeforePipeline();
            this.OnError = new ErrorPipeline();

            this.ModulePath = modulePath;
        }

        private IEnumerable<Route> GetRoutes()
        {
            // Run through all the methods on the class looking
            // for our attribute. If we were to do this for a real
            // app we'd be checking parameters and return types etc
            // but for simplicity we won't bother here.
            var routes = new List<Route>();
            var type = this.GetType();

            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttributes(typeof(NancyRouteAttribute), false).FirstOrDefault() as NancyRouteAttribute;

                if (attribute == null)
                {
                    continue;
                }

                var routeDelegate = WrapFunc((Func<dynamic, dynamic>)Delegate.CreateDelegate(typeof(Func<dynamic, dynamic>), this, method.Name));

                var filter = this.GetFilter(method.Name);

                var fullPath = String.Concat(this.ModulePath, attribute.Path);

                routes.Add(new Route(attribute.Method.ToUpper(), fullPath, filter, routeDelegate));
            }

            return routes.AsReadOnly();
        }

        private Func<NancyContext, bool> GetFilter(string routeMethodName)
        {
            var type = this.GetType();
            var method = type.GetMethod(routeMethodName + "Filter", BindingFlags.Public | BindingFlags.Instance);

            if (method == null)
            {
                return null;
            }

            return (Func<NancyContext, bool>)Delegate.CreateDelegate(typeof(Func<NancyContext, bool>), this, method.Name);
        }

        /// <summary>
        /// Wraps a sync delegate in a delegate that returns a task
        /// </summary>
        /// <param name="syncFunc">Sync delegate</param>
        /// <returns>Task wrapped version</returns>
        private static Func<dynamic, CancellationToken, Task<dynamic>> WrapFunc(Func<object, object> syncFunc)
        {
            return (p, ct) =>
            {
                var tcs = new TaskCompletionSource<dynamic>();

                try
                {
                    var result = syncFunc.Invoke(p);

                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }

                return tcs.Task;
            };
        }
    }
}