namespace Nancy.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Collection class for static culture conventions
    /// </summary>
    public class CultureConventions : IEnumerable<Func<NancyContext, CultureInfo>>
    {
        private readonly IEnumerable<Func<NancyContext, CultureInfo>> conventions;

        /// <summary>
        /// Creates a new instance of CultureConventions
        /// </summary>
        /// <param name="conventions"></param>
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
