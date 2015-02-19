namespace Nancy.Testing
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using CsQuery;
    using CsQuery.Implementation;

    public class QueryWrapper : IEnumerable<NodeWrapper>
    {
        private readonly CQ document;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryWrapper"/> class, using
        /// the provided <paramref name="document"/>.
        /// </summary>
        /// <param name="document">The document that should be wrapped.</param>
        public QueryWrapper(CQ document)
        {
            this.document = document;
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
                var newDocument = this.document.Selector == null
                                      ? this.document
                                      : new CQ(this.document.RenderSelection());
                return new QueryWrapper(newDocument[selector]);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<NodeWrapper> GetEnumerator()
        {
            return this.document.Select(x => new NodeWrapper(x as DomElement)).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CQ"/> to <see cref="QueryWrapper"/>.
        /// </summary>
        /// <param name="document">The <see cref="CQ"/> that should be cast.</param>
        /// <returns>An <see cref="QueryWrapper"/> instance, that contains the results of the cast.</returns>
        public static implicit operator QueryWrapper(CQ document)
        {
            return new QueryWrapper(document);
        }
    }
}