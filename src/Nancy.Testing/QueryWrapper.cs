namespace Nancy.Testing
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AngleSharp.Dom;

    /// <summary>
    /// Simple wrapper around a collection of <see cref="IElement"/> instances.
    /// </summary>
    public class QueryWrapper : IEnumerable<NodeWrapper>
    {
        private readonly IReadOnlyCollection<IElement> elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryWrapper"/> class, using
        /// the provided <paramref name="elements"/>.
        /// </summary>
        /// <param name="elements">The elements that were the result of query.</param>
        public QueryWrapper(IReadOnlyCollection<IElement> elements)
        {
            this.elements = elements;
        }

        /// <summary>
        /// Gets elements from CSS3 selectors
        /// </summary>
        /// <param name="selector">CSS3 selector</param>
        /// <returns>A <see cref="QueryWrapper"/> instance</returns>
        public QueryWrapper this[string selector]
        {
            get
            {
                return new QueryWrapper(this.elements.SelectMany(element => element.QuerySelectorAll(selector)).ToArray());
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<NodeWrapper> GetEnumerator()
        {
            return this.elements.Select(element => new NodeWrapper(element)).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
