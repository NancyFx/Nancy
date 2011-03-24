namespace Nancy.Testing
{
    using System;

    public class IndexHelper<TKey, TValue>
    {
        private Func<TKey, TValue> indexDelegate;

        public TValue this[TKey key]
        {
            get
            {
                return this.indexDelegate(key);
            }
        }

        public IndexHelper(Func<TKey, TValue> indexDelegate)
        {
            this.indexDelegate = indexDelegate;
        }
    }
}