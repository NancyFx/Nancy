namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;

    public interface IRouteResolver
    {
        IRoute GetRoute(IRequest request, IEnumerable<RouteDescription> descriptions);
    }

    public class RouteResolver : IRouteResolver
    {
        public IRoute GetRoute(IRequest request, IEnumerable<RouteDescription> descriptions)
        {
            throw new NotImplementedException();
        }
    }
}