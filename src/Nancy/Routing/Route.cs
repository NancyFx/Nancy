namespace Nancy.Routing
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the core functionality of a route.
    /// </summary>
    public abstract class Route
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> type, with the specified <see cref="RouteDescription"/>.
        /// </summary>
        /// <param name="description">An <see cref="RouteDescription"/> instance.</param>
        protected Route(RouteDescription description)
        {
            this.Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route{T}"/> type, with the specified definition.
        /// </summary>
        /// <param name="name">Route name</param>
        /// <param name="method">The HTTP method that the route is declared for.</param>
        /// <param name="path">The path that the route is declared for.</param>
        /// <param name="condition">A condition that needs to be satisfied inorder for the route to be eligible for invocation.</param>
        /// <param name="returnType">The <see cref="Type"/> of the value returned by the route.</param>
        protected Route(string name, string method, string path, Func<NancyContext, bool> condition, Type returnType)
            : this(new RouteDescription(name, method, path, condition, returnType))
        {
        }

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
        /// <returns>The value that was produced by the route.</returns>
        public abstract Task<object> Invoke(DynamicDictionary parameters, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Stores information about a declared route in Nancy.
    /// </summary>
    [DebuggerDisplay("{Description.DebuggerDisplay, nq}")]
    public class Route<T> : Route
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Route{T}"/> type, with the specified <see cref="RouteDescription"/>.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="action">The action that should take place when the route is invoked.</param>
        public Route(RouteDescription description, Func<object, CancellationToken, Task<T>> action)
            : base(description)
        {
            this.Action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route{T}"/> type, with the specified definition.
        /// </summary>
        /// <param name="name">Route name</param>
        /// <param name="method">The HTTP method that the route is declared for.</param>
        /// <param name="path">The path that the route is declared for.</param>
        /// <param name="condition">A condition that needs to be satisfied inorder for the route to be eligible for invocation.</param>
        /// <param name="action">The action that should take place when the route is invoked.</param>
        public Route(string name, string method, string path, Func<NancyContext, bool> condition, Func<object, CancellationToken, Task<T>> action)
            : this(new RouteDescription(name, method, path, condition, typeof(T)), action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Route{T}"/> type, with the specified definition.
        /// </summary>
        /// <param name="method">The HTTP method that the route is declared for.</param>
        /// <param name="path">The path that the route is declared for.</param>
        /// <param name="condition">A condition that needs to be satisfied inorder for the route to be eligiable for invocation.</param>
        /// <param name="action">The action that should take place when the route is invoked.</param>
        public Route(string method, string path, Func<NancyContext, bool> condition, Func<object, CancellationToken, Task<T>> action)
            : this(string.Empty, method, path, condition, action)
        {
        }

        /// <summary>
        /// Gets or sets the action that should take place when the route is invoked.
        /// </summary>
        /// <value>A <see cref="Func{T,K}"/> that represents the action of the route.</value>
        public Func<object, CancellationToken, Task<T>> Action { get; set; }

        /// <summary>
        /// Invokes the route with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">A <see cref="DynamicDictionary"/> that contains the parameters that should be passed to the route.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A (hot) task of <see cref="Response"/> instance.</returns>
        public override async Task<object> Invoke(DynamicDictionary parameters, CancellationToken cancellationToken)
        {
            return await this.Action.Invoke(parameters, cancellationToken).ConfigureAwait(false); 
        }
    }
}
