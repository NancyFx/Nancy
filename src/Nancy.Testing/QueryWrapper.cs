namespace Nancy.Testing
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using HtmlAgilityPlus;

    public class QueryWrapper : IEnumerable<NodeWrapper>
    {
        private SharpQuery query;

        private QueryWrapper(SharpQuery query)
        {
            this.query = query;
        }

        /// <summary>
        /// Gets elements from CSS3 selectors
        /// </summary>
        /// <param name="selector">CSS3 selector</param>
        /// <returns>QueryWrapper instance</returns>
        public QueryWrapper this[string selector]
        {
            get
            {
                return this.query.Find(selector);
            }
        }

        public static implicit operator QueryWrapper(SharpQuery sourceQuery)
        {
            return new QueryWrapper(sourceQuery);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<NodeWrapper> GetEnumerator()
        {
            return this.query.All().Select(n => (NodeWrapper)n).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}