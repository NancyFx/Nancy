namespace Nancy.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Collection class for static culture conventions
    /// </summary>
    public class CultureConventions : IEnumerable<Func<NancyContext, GlobalizationConfiguration, CultureInfo>>
    {
        private readonly IEnumerable<Func<NancyContext, GlobalizationConfiguration, CultureInfo>> conventions;

        /// <summary>
        /// Creates a new instance of CultureConventions
        /// </summary>
        /// <param name="conventions"></param>
        public CultureConventions(IEnumerable<Func<NancyContext, GlobalizationConfiguration, CultureInfo>> conventions)
        {
            this.conventions = conventions;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Func<NancyContext, GlobalizationConfiguration, CultureInfo>> GetEnumerator()
        {
            return conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
