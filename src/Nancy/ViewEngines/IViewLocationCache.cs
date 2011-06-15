namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    public interface IViewLocationCache : IEnumerable<ViewLocationResult>
    {
    }
}