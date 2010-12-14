namespace Nancy.Tests.Fakes
{    
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Routing;

    public class FakeRouteResolver : IRouteResolver
    {
        public IRoute GetRoute(IRequest request, IEnumerable<ModuleMeta> meta, INancyApplication application)
        {
            var description = (from m in meta
                               from d in m.RouteDescriptions
                               where d.ModulePath + d.Path == request.Uri
                               select d).First();

            this.ModulePath = description.Module.ModulePath;
            this.Path = description.Path;            

            return new FakeRoute();
        }

        
        public string Path { get; private set; }

        public string ModulePath { get; private set; }
    }
}
