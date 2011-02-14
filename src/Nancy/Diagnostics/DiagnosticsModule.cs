namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Routing;
    using System.Text;

    public class DiagnosticsModule : NancyModule
    {
        private readonly IRouteCacheProvider routeCacheProvider;

        public DiagnosticsModule(IRouteCacheProvider routeCacheProvider) : base("/nancy/diagnostics")
        {
            this.routeCacheProvider = routeCacheProvider;
            Get["/"] = parameters => {

                var routesPerMethod = this.GetRoutesPerMethod();

                var output = new StringBuilder();

                output.Append("<html>");
                output.Append("<body>");

                foreach (var method in routesPerMethod)
                {
                    output.Append(this.GetGroupedRoutes(method));
                }

                output.Append("</body>");
                output.Append("</html>");

                return output.ToString();
            };
        }


        private string GetGroupedRoutes(IGrouping<string, RouteCacheEntry> grouped)
        {
            var builder = new StringBuilder();

            builder.Append("<dl>");
            builder.AppendFormat("<dt>{0}</dt>", grouped.Key);
            builder.Append("<dd>");
            builder.Append("<ul>");

            foreach (var routeCacheEntry in grouped)
            {
                builder.AppendFormat("<li>{0}</li>", routeCacheEntry.Path);
            }

            builder.Append("<ul>");
            builder.Append("<dd>");
            builder.Append("</dl>");

            return builder.ToString();
        }


        private IEnumerable<IGrouping<string, RouteCacheEntry>> GetRoutesPerMethod()
        {
            var routes =
                from route in this.routeCacheProvider.GetCache()
                group route by route.Method;

            return routes;
        }
    }
}