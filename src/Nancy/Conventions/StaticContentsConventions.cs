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

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticContentsConventions"/> class.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        public StaticContentsConventions(IEnumerable<Func<NancyContext, string, Response>> conventions)
        {
            this.conventions = conventions;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
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