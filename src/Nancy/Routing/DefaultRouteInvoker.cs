namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Default route invoker implementation-
    /// </summary>
    public class DefaultRouteInvoker : IRouteInvoker
    {
        private readonly IEnumerable<ISerializer> serializers;

        public DefaultRouteInvoker(IEnumerable<ISerializer> serializers)
        {
            this.serializers = serializers;
        }

        /// <summary>
        /// Invokes the specified <paramref name="route"/> with the provided <paramref name="parameters"/>.
        /// </summary>
        /// <param name="route">The route that should be invoked.</param>
        /// <param name="parameters">The parameters that the route should be invoked with.</param>
        /// <param name="context">The context of the route that is being invoked.</param>
        /// <returns>A <see cref="Response"/> intance that represents the result of the invoked route.</returns>
        public Response Invoke(Route route, DynamicDictionary parameters, NancyContext context)
        {
            var result =
                route.Invoke(parameters);

            var response =
                CastResultToResponse(result) ?? GetNegotiatedResponse(result, context);

            return response;
        }

        private static Response CastResultToResponse(dynamic result)
        {
            return result as Response;
        }

        private Response GetNegotiatedResponse(dynamic result, NancyContext context)
        {
            var headers =
                context.Request.Headers;

            var valid =
                from accept in headers.Accept
                where accept.Item2 > 0m
                let s = this.serializers.FirstOrDefault(s => s.CanSerialize(accept.Item1))
                where s != null
                select Tuple.Create(accept.Item1, s);

            var serializer =
                valid.FirstOrDefault();

            var response = new Response {
                ContentType = serializer.Item1,
                StatusCode = HttpStatusCode.OK,
                Contents = s => serializer.Item2.Serialize(serializer.Item1, result, s)
            };

            if (valid.Count() > 1)
            {
                response.WithHeader("Vary", "Accept");
            }

            return response;
        }
    }
}