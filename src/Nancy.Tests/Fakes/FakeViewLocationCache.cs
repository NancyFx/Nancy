namespace Nancy.Tests.Fakes
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.ViewEngines;

    public class FakeViewLocationCache : IViewLocationCache
    {
        private readonly List<ViewLocationResult> cache;

        public FakeViewLocationCache(params ViewLocationResult[] results)
        {
            this.cache = results.ToList();
        }

        public IEnumerator<ViewLocationResult> GetEnumerator()
        {
            return this.cache.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}