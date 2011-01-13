namespace Nancy.Tests.Fakes
{    
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Routing;

    public class FakeRouteResolver : IRouteResolver
    {
        public IRoute GetRoute(IRequest request)
        {
            return new FakeRoute();
        }
        
        public string Path { get; private set; }

        public string ModulePath { get; private set; }

    }
}
