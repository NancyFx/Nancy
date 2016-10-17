using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Nancy.ViewEngines.Razor
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            // Nancy is handling view path expansion, so always just look at the view name
            return new[] { "{0}" };
        }
    }
}