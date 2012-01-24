namespace Nancy.Routing
{
    using System;

    /// <summary>
    /// Represents the various parts of a route lambda.
    /// </summary>
    public sealed class RouteDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDescription"/> class.
        /// </summary>
        /// <param name="method">The request method of the route.</param>
        /// <param name="path">The path that the route will be invoked for.</param>
        /// <param name="condition">The condition that has to be fulfilled for the route to be a valid match.</param>
        public RouteDescription(string method, string path, Func<NancyContext, bool> condition)
        {
            if (String.IsNullOrEmpty(method))
            {
                throw new ArgumentException("Method must be specified", method);
            }

            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path must be specified", method);
            }

            this.Method = method;
            this.Path = path;
            this.Condition = condition;
        }

        /// <summary>
        /// The condition that has to be fulfilled inorder for the route to be a valid match.
        /// </summary>
        /// <value>A function that evaluates the condition when a <see cref="NancyContext"/> instance is passed in.</value>
        public Func<NancyContext, bool> Condition { get; private set; }

        /// <summary>
        /// Gets the method of the route.
        /// </summary>
        /// <value>A <see cref="string"/> containing the method of the route.</value>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the path that the route will be invoked for.
        /// </summary>
        /// <value>A <see cref="string"/> containing the path of the route.</value>
        public string Path { get; private set; }
    }
}