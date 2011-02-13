namespace Nancy.Routing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class RouteCollection : IEnumerable<Route>
    {
        private List<Route> routes = new List<Route>();

        public string Method { get; private set; }

        public RouteCollection(string method)
        {
            this.Method = method;
        }

        /// <summary>
        /// Gets or sets the action for a specific <param name="route"/>.
        /// </summary>
        /// <value>A <see cref="Func{T, K}"/> containing the action declaration for specified <param name="route"/></value>
        /// <remarks>Actions that are declared with this indexer will get a condition that is always <see langword="null"/>.</remarks>
        public Func<dynamic, Response> this[string route]
        {
            set { this.AddRoute(route, null, value); }
        }

        /// <summary>
        /// Gets or sets the action for a specific <param name="route"/>.
        /// </summary>
        /// <value>A <see cref="Func{T, K}"/> containing the action declaration for specified <param name="route"/> and <paramref name="condition"/>.</value>
        public Func<dynamic, Response> this[string route, Func<Request, bool> condition]
        {
            set { this.AddRoute(route, condition, value); }
        }

        private void AddRoute(string route, Func<Request, bool> condition, Func<dynamic, Response> action)
        {
            this.routes.Add(new Route(this.Method, route, condition, action, new DynamicDictionary()));
        }

        public Route GetRouteByIndex(int index)
        {
            return this.routes[index];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Route> GetEnumerator()
        {
            return routes.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this.routes as IEnumerable).GetEnumerator();
        }
    }
}