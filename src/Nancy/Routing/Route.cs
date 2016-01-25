namespace Nancy.Routing
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Stores information about a declared route in Nancy.
    /// </summary>
    [DebuggerDisplay("{Description.DebuggerDisplay, nq}")]
    public class Route
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> type, with the specified <see cref="RouteDescription"/>.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="action">The action that should take place when the route is invoked.</param>
        public Route(RouteDescription description, Func<dynamic, CancellationToken, Task<dynamic>> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.Description = description;
            this.Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> type, with the specified definition.
        /// </summary>
        /// <param name="name">Route name</param>
        /// <param name="method">The HTTP method that the route is declared for.</param>
        /// <param name="path">The path that the route is declared for.</param>
        /// <param name="condition">A condition that needs to be satisfied inorder for the route to be eligible for invocation.</param>
        /// <param name="action">The action that should take place when the route is invoked.</param>
        public Route(string name, string method, string path, Func<NancyContext, bool> condition, Func<dynamic, CancellationToken, Task<dynamic>> action)
            : this(new RouteDescription(name, method, path, condition), action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> type, with the specified definition.
        /// </summary>
        /// <param name="method">The HTTP method that the route is declared for.</param>
        /// <param name="path">The path that the route is declared for.</param>
        /// <param name="condition">A condition that needs to be satisfied inorder for the route to be eligiable for invocation.</param>
        /// <param name="action">The action that should take place when the route is invoked.</param>
        public Route(string method, string path, Func<NancyContext, bool> condition, Func<dynamic, CancellationToken, Task<dynamic>> action)
            : this(string.Empty, method, path, condition, action)
        {
        }

        /// <summary>
        /// Gets or sets the action that should take place when the route is invoked.
        /// </summary>
        /// <value>A <see cref="Func{T,K}"/> that represents the action of the route.</value>
        public Func<dynamic, CancellationToken, Task<dynamic>> Action { get; set; }

        /// <summary>
        /// Gets the description of the route.
        /// </summary>
        /// <value>A <see cref="RouteDescription"/> instance.</value>
        public RouteDescription Description { get; private set; }

        /// <summary>
        /// Invokes the route with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">A <see cref="DynamicDictionary"/> that contains the parameters that should be passed to the route.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A (hot) task of <see cref="Response"/> instance.</returns>
        public Task<dynamic> Invoke(DynamicDictionary parameters, CancellationToken cancellationToken)
        {
            return this.Action.Invoke(parameters, cancellationToken);
        }

        /// <summary>
        /// Creates a route from a sync delegate signature
        /// </summary>
        /// <param name="description"></param>
        /// <param name="syncFunc">The action that should take place when the route is invoked.</param>
        /// <returns>A Route instance</returns>
        [Obsolete("Sync routes are deprecated (see LegacyNancyModule)")]
        public static Route FromSync(RouteDescription description, Func<dynamic, dynamic> syncFunc)
        {
            return new Route(description, Wrap(syncFunc));
        }

        /// <summary>
        /// Creates a route from a sync delegate signature
        /// </summary>
        /// <param name="method">The HTTP method that the route is declared for.</param>
        /// <param name="path">The path that the route is declared for.</param>
        /// <param name="condition">A condition that needs to be satisfied inorder for the route to be eligiable for invocation.</param>
        /// <param name="syncFunc">The action that should take place when the route is invoked.</param>
        /// <returns>A Route instance</returns>
        [Obsolete("Sync routes are deprecated (see LegacyNancyModule)")]
        public static Route FromSync(string method, string path, Func<NancyContext, bool> condition, Func<dynamic, dynamic> syncFunc)
        {
            return FromSync(string.Empty, method, path, condition, syncFunc);
        }

        /// <summary>
        /// Creates a route from a sync delegate signature
        /// </summary>
        /// <param name="name">Route name</param>
        /// <param name="method">The HTTP method that the route is declared for.</param>
        /// <param name="path">The path that the route is declared for.</param>
        /// <param name="condition">A condition that needs to be satisfied inorder for the route to be eligible for invocation.</param>
        /// <param name="syncFunc">The action that should take place when the route is invoked.</param>
        /// <returns>A Route instance</returns>
        [Obsolete("Sync routes are deprecated (see LegacyNancyModule)")]
        public static Route FromSync(string name, string method, string path, Func<NancyContext, bool> condition, Func<dynamic, dynamic> syncFunc)
        {
            return FromSync(new RouteDescription(name, method, path, condition), syncFunc);
        }

        /// <summary>
        /// Wraps a sync delegate in a delegate that returns a task
        /// </summary>
        /// <param name="syncFunc">Sync delegate</param>
        /// <returns>Task wrapped version</returns>
        private static Func<dynamic, CancellationToken, Task<dynamic>> Wrap(Func<object, object> syncFunc)
        {
            return (parameters, context) =>
                {
                    var tcs = new TaskCompletionSource<dynamic>();

                    try
                    {
                        var result = syncFunc.Invoke(parameters);

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
