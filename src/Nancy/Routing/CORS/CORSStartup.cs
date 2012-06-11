namespace Nancy.Routing.CORS
{
    using System;
    using System.Collections.Generic;
    
    using Nancy.Bootstrapper;
    
    public class CORSStartup : IStartup
    {
        private readonly IRoutePatternMatcher _routePatternMatcher;

        public CORSStartup(IRoutePatternMatcher routePatternMatcher)
        {
            _routePatternMatcher = routePatternMatcher;
        }

        #region Implementation of IStartup

        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get { return null; }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get { return null;  }
        }

        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get { return null; }
        }

        public void Initialize(IPipelines pipelines)
        {
            CORSModuleExtension.RoutePatternMatcher = _routePatternMatcher;
        }

        #endregion
    }
}