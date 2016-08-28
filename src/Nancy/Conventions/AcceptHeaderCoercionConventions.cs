namespace Nancy.Conventions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Collection of accept header coercions
    /// </summary>
    public class AcceptHeaderCoercionConventions : IEnumerable<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>>
    {
        private readonly IList<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>> conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AcceptHeaderCoercionConventions"/> class.
        /// </summary>
        /// <param name="conventions">The conventions.</param>
        public AcceptHeaderCoercionConventions(IList<Func<IEnumerable<Tuple<string, decimal>>, NancyContext, IEnumerable<Tuple<string, decimal>>>> conventions)
        {
            this.conventions = conventions;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
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