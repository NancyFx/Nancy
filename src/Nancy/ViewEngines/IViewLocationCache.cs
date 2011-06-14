namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;

    public interface IViewLocationCache
    {
        IEnumerable<ViewLocationResult> GetMatchingViews(Func<ViewLocationResult, bool> criterion);
    }
}