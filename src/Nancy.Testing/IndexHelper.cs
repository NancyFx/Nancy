namespace Nancy.Testing
{
    using System;

    /// <summary>
    /// A helper class for providing classes with "named indexers".
    /// </summary>
    /// <typeparam name="TKey">The indexer key type</typeparam>
    /// <typeparam name="TValue">The indexer return value type.</typeparam>
    public class IndexHelper<TKey, TValue>
    {
        private readonly Func<TKey, TValue> indexDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexHelper&lt;TKey, TValue&gt;"/> class.
        /// </summary>
        /// <param name="indexDelegate">The index delegate.</param>
        public IndexHelper(Func<TKey, TValue> indexDelegate)
        {
            this.indexDelegate = indexDelegate;
        }

        /// <summary>
        /// Gets the <typeparamref name="TValue"/> with the specified key.
        /// </summary>
        /// <value>The value of the indexer.</value>
        public TValue this[TKey key]
        {
            get { return this.indexDelegate(key); }
        }
    }
}