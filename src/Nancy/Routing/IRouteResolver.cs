namespace Nancy.Routing
{
    using System.Collections.Generic;

    public interface IRouteResolver
    {
        IRoute GetRoute(IRequest request, IEnumerable<NancyModule> modules, ITemplateEngineSelector application);
    }
}