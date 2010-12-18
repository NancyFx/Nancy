namespace Nancy.Routing
{
    using System.Collections.Generic;

    public interface IRouteResolver
    {
        IRoute GetRoute(IRequest request, IEnumerable<ModuleMeta> metas, INancyApplication application);
    }
}