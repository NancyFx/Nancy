namespace Nancy.Routing.CORS
{
    using System;
    using System.Collections.Generic;
    
    using Nancy.Bootstrapper;
    
    public class CORSStartup : IApplicationStartup
    {
        private readonly IRoutePatternMatcher _routePatternMatcher;

        public CORSStartup (IRoutePatternMatcher routePatternMatcher)
        {
            _routePatternMatcher = routePatternMatcher;
        }

        #region Implementation of IApplicationStartup

        public void Initialize(IPipelines pipelines)
        {
            CORSModuleExtension.RoutePatternMatcher = _routePatternMatcher;
        }

        #endregion
    }
}