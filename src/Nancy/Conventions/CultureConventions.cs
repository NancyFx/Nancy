namespace Nancy.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;


    /// <summary>
    /// Collection class for static culture conventions
    /// </summary>
    /// <seealso cref="Func{TContext, TGlobalizationConfiguration, TResult}" />
    public class CultureConventions : IEnumerable<Func<NancyContext, GlobalizationConfiguration, CultureInfo>>
    {
        private readonly IEnumerable<Func<NancyContext, GlobalizationConfiguration, CultureInfo>> conventions;


        /// <summary>
        /// Initializes a new instance of the <see cref="CultureConventions"/> class, with
        /// the provided <paramref name="conventions"/>.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
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
            return this.conventions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
