namespace Nancy.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Collection class for static content conventions
    /// </summary>
    public class StaticContentsConventions : IEnumerable<Func<NancyContext, string, Response>>
    {
        private readonly IEnumerable<Func<NancyContext, string, Response>> conventions;

        public StaticContentsConventions(IEnumerable<Func<NancyContext, string, Response>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<NancyContext, string, Response>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}