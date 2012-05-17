namespace Nancy.Routing
{
    public interface IRouteInvoker
    {
        Response Invoke(Route route, DynamicDictionary parameters);
    }

    public class DefaultRouteInvoker : IRouteInvoker
    {
        public Response Invoke(Route route, DynamicDictionary parameters)
        {
            var result =
                route.Invoke(parameters);

            return result;
        }
    }
}