using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nancy.Conventions
{
    public class CultureConventions : IEnumerable<Func<NancyContext, CultureInfo>>
    {
        private readonly IEnumerable<Func<NancyContext, CultureInfo>> conventions;

        public CultureConventions(IEnumerable<Func<NancyContext, CultureInfo>> conventions)
        {
            this.conventions = conventions;
        }

        public IEnumerator<Func<NancyContext, CultureInfo>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
