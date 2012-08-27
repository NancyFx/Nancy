using System;
using System.Collections;
using System.Collections.Generic;

namespace Nancy.Conventions
{
    public class AcceptHeaderCoercionConventions : IEnumerable<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>>
    {
        private readonly IList<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>> conventions;

        public AcceptHeaderCoercionConventions(IList<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>> GetEnumerator()
        {
            return this.conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}