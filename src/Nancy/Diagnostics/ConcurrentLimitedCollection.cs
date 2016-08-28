namespace Nancy.Diagnostics
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a thread safe, limited size, collection of objects
    /// If the collection is full the oldest item gets removed.
    /// </summary>
    /// <typeparam name="T">Type to store</typeparam>
    public class ConcurrentLimitedCollection<T> : IEnumerable<T>
    {
        private readonly int maxSize;

        private ConcurrentQueue<T> internalStore;

        /// <summary>
        /// Gets the current size for the collection.
        /// </summary>
        /// <value>
        /// The size of the current.
        /// </value>
        public int CurrentSize
        {
            get
            {
                return this.internalStore.Count;
            }
        }

        /// <summary>
        /// Creates a new instance of the ConcurrentLimitedCollection class
        /// </summary>
        /// <param name="maxSize">Maximum size for the collection</param>
        public ConcurrentLimitedCollection(int maxSize)
        {
            this.maxSize = maxSize;
            this.internalStore = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator()
        {
            return this.internalStore.GetEnumerator();
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

        /// <summary>
        /// Adds an item to the collection.
        /// If the collection is full, the oldest item is removed and the new item
        /// is added to the end of the collection.
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(T item)
        {
            if (this.internalStore.Count == this.maxSize)
            {
                T temp;
                this.internalStore.TryDequeue(out temp);
            }

            this.internalStore.Enqueue(item);
        }

        /// <summary>
        /// Clear the collection
        /// </summary>
        public void Clear()
        {
            this.internalStore = new ConcurrentQueue<T>();
        }
    }
}