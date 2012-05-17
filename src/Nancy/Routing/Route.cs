namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// Stores information about a declared route in Nancy.
    /// </summary>
    public class Route
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Route"/> type, with the specified <see cref="RouteDescription"/>.
        /// </summary>
        /// <param name="description"></param>
        /// <param name="action">The action that should take place when the route is invoked.</param>
        public Route(RouteDescription description, Func<dynamic, dynamic> action)
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
        /// <param name="method">The HTTP method that the route is declared for.</param>
        /// <param name="path">The path that the route is declared for.</param>
        /// <param name="condition">A condition that needs to be satisfied inorder for the route to be eligiable for invocation.</param>
        /// <param name="action">The action that should take place when the route is invoked.</param>
        public Route(string method, string path, Func<NancyContext, bool> condition, Func<dynamic, dynamic> action)
            : this(new RouteDescription(method, path, condition), action)
        {
        }

        /// <summary>
        /// Gets or sets the action that should take place when the route is invoked.
        /// </summary>
        /// <value>A <see cref="Func{T,K}"/> that represents the action of the route.</value>
        public Func<dynamic, dynamic> Action { get; set; }

        /// <summary>
        /// Gets the description of the route.
        /// </summary>
        /// <value>A <see cref="RouteDescription"/> instance.</value>
        public RouteDescription Description { get; private set; }

        /// <summary>
        /// Invokes the route with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="parameters">A <see cref="DynamicDictionary"/> that contains the parameters that should be passed to the route.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public dynamic Invoke(DynamicDictionary parameters)
        {
            return this.Action.Invoke(parameters);
        }
    }
}
