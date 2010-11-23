namespace Nancy
{
    using System;
    using System.Collections.Generic;

    public interface IRouteResolver
    {
        IRoute GetRoute(IRequest request, IEnumerable<INancy> modules);
    }

    public class RouteResolver : IRouteResolver
    {
        public IRoute GetRoute(IRequest request, IEnumerable<INancy> modules)
        {
            throw new NotImplementedException();
        }
    }
}