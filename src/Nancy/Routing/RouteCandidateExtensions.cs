namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods used by the <see cref="DefaultRouteResolver"/>.
    /// </summary>
    public static class RouteCandidateExtensions
    {
        /// <summary>
        /// Converts an <see cref="IRouteCache"/> into the format used by diagnostics to store route parsing results.
        /// </summary>
        /// <param name="cache">The cache that should be converted.</param>
        /// <returns>A tuple of valid and rejected routes.</returns>
        public static Tuple<List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>, Dictionary<string, List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>>> GetRouteCandidates(this IRouteCache cache)
        {
            var result = new Tuple<List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>, Dictionary<string, List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>>>(
                new List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>(),
                new Dictionary<string, List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>>()
                );

            foreach (var cacheEntry in cache)
            {
                foreach (var candidate in cacheEntry.Value.Select(cacheEntryRoutes => new Tuple<string, int, RouteDescription, IRoutePatternMatchResult>(cacheEntry.Key, cacheEntryRoutes.Item1, cacheEntryRoutes.Item2, null)))
                {
                    result.Item1.Add(candidate);
                }
            }

            return result;
        }

        /// <summary>
        /// Applies a filter to the provided list of routes. Routes that do not pass the filter will be places in the second item of the returned tuple
        /// along with the provided <paramref name="rejectReason"/>.
        /// </summary>
        /// <param name="routes">The routes that the filter should be applied to.</param>
        /// <param name="context">The context of the current request.</param>
        /// <param name="rejectReason">The message that should be attached to rejected routes.</param>
        /// <param name="condition">The condition that routes need to fulfill.</param>
        /// <returns>A tuple of valid and rejected routes.</returns>
        public static Tuple<List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>, Dictionary<string, List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>>> Filter(this Tuple<List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>, Dictionary<string, List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>>> routes, NancyContext context, string rejectReason, Func<NancyContext, Tuple<string, int, RouteDescription, IRoutePatternMatchResult>, Tuple<bool, Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>> condition)
        {
            var result = new Tuple<List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>, Dictionary<string, List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>>>(
                new List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>(),
                routes.Item2
                );

            foreach (var conditionResult in routes.Item1.Select(route => condition.Invoke(context, route)))
            {
                if (conditionResult.Item1)
                {
                    result.Item1.Add(conditionResult.Item2);
                }
                else
                {
                    if (!result.Item2.ContainsKey(rejectReason))
                    {
                        result.Item2.Add(rejectReason, new List<Tuple<string, int, RouteDescription, IRoutePatternMatchResult>>());
                    }

                    result.Item2[rejectReason].Add(conditionResult.Item2);
                }
            }

            return result;
        }
    }
}