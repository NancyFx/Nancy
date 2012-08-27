using System;
using System.Collections;
using System.Collections.Generic;

namespace Nancy.Conventions
{
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