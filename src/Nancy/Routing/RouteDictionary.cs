namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RouteDictionary
    {
        private readonly Dictionary<Tuple<string, Func<Request, bool>>, Func<dynamic, Response>> routes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDictionary"/> class.
        /// </summary>
        /// <param name="module"></param>
        public RouteDictionary(NancyModule module, string method)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module", "The value of the module parameter cannot be null.");
            }

            if (method == null)
            {
                throw new ArgumentNullException("method", "The value of the method parameter cannot be null.");
            }

            if (method.Length == 0)
            {
                throw new ArgumentOutOfRangeException("method", string.Empty, "The value of the method parameter cannot be empty.");
            }

            this.Method = method;
            this.Module = module;
            this.routes = new Dictionary<Tuple<string, Func<Request, bool>>, Func<dynamic, Response>>(new RouteDictionaryEqualityComparer());
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>The method.</value>
        public string Method { get; private set; }

        /// <summary>
        /// Gets the <see cref="NancyModule"/> instance that the routes are registred in.
        /// </summary>
        /// <value>A <see cref="NancyModule"/> instance.</value>
        public NancyModule Module { get; private set; }

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
            var compositeKey =
                new Tuple<string, Func<Request, bool>>(route, condition);

            this.routes[compositeKey] = action;
        }

        /// <summary>
        /// Gets the <see cref="RouteDescription"/> instances that describes the routes that are stored.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> containing <see cref="RouteDescription"/> instances.</returns>
        public IEnumerable<RouteDescription> GetRouteDescriptions()
        {
            var descriptions =
                from candidate in this.routes.Keys
                select new RouteDescription()
                {
                    Action = this.routes[candidate],
                    Condition = candidate.Item2,
                    Method = this.Method,
                    Module = this.Module,
                    Path = string.Concat(this.Module.ModulePath, candidate.Item1)
                };

            return descriptions.ToList();
        }

        // TODO - This needs work so it doesn't pick filtered routes
        public RouteDescription GetRoute(string route)
        {
            var descriptions =
                from candidate in this.routes.Keys
                where string.Concat(this.Module.ModulePath, candidate.Item1).Equals(route, StringComparison.OrdinalIgnoreCase)
                select new RouteDescription()
                {
                    Action = this.routes[candidate],
                    Condition = candidate.Item2,
                    Method = this.Method,
                    Module = this.Module,
                    Path = string.Concat(this.Module.ModulePath, candidate.Item1)
                };

            return descriptions.FirstOrDefault();
        }

        /// <summary>
        /// Equality comparer for the composite key used in the route dictionary.
        /// </summary>
        private class RouteDictionaryEqualityComparer : IEqualityComparer<Tuple<string, Func<Request, bool>>>
        {
            public bool Equals(Tuple<string, Func<Request, bool>> x, Tuple<string, Func<Request, bool>> y)
            {
                return x.Item1.Equals(y.Item1, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(Tuple<string, Func<Request, bool>> obj)
            {
                unchecked
                {
                    var result = (obj.Item1 != null ? obj.Item1.GetHashCode() : 0);
                    result = (result * 397) ^ (obj.Item2 != null ? obj.Item2.GetHashCode() : 0);
                    return result;
                }
            }
        }
    }
}