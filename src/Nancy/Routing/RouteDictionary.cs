namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RouteInformation
    {
        public Func<dynamic, Response> Action { get; set; }

        public Func<bool> Condition { get; set; }

        public string Route { get; set; }
    }

    public class RouteDictionary
    {
        private readonly Dictionary<Tuple<string, Func<bool>>, Func<dynamic, Response>> routes;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDictionary"/> class.
        /// </summary>
        /// <param name="module"></param>
        public RouteDictionary(NancyModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException("module", "The value of the module parameter cannot be null.");
            }

            this.Module = module;
            this.routes = new Dictionary<Tuple<string, Func<bool>>, Func<dynamic, Response>>(new RouteDictionaryEqualityComparer());
        }

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
        public Func<dynamic, Response> this[string route, Func<bool> condition]
        {
            set { this.AddRoute(route, condition, value); }
        }

        private void AddRoute(string route, Func<bool> condition, Func<dynamic, Response> action)
        {
            var compositeKey =
                new Tuple<string, Func<bool>>(route, condition);

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
                    Module = this.Module,
                    Path = candidate.Item1
                };

            return descriptions;
        }

        public RouteInformation GetRoute(string route)
        {
            var matches =
                from candidate in this.routes.Keys
                where candidate.Item1.Equals(route, StringComparison.OrdinalIgnoreCase)
                select new RouteInformation
                {
                    Action = this.routes[candidate],
                    Condition = candidate.Item2,
                    Route = string.Concat(this.Module.ModulePath, candidate.Item1)
                };

            return matches.FirstOrDefault();
        }

        /// <summary>
        /// Equality comparer for the composite key used in the route dictionary.
        /// </summary>
        private class RouteDictionaryEqualityComparer : IEqualityComparer<Tuple<string, Func<bool>>>
        {
            public bool Equals(Tuple<string, Func<bool>> x, Tuple<string, Func<bool>> y)
            {
                return x.Item1.Equals(y.Item1, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(Tuple<string, Func<bool>> obj)
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