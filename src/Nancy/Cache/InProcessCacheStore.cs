namespace Nancy.Cache
{
    using System.Collections.Concurrent;

    /// <summary>
    /// An in-process implementation of <see cref="ICacheStore"/>
    /// </summary>
    public class InProcessCacheStore : ICacheStore
    {
        private static ConcurrentDictionary<string, object> items = new ConcurrentDictionary<string, object>();

        public bool TryLoad<T>(string id, out T obj)
        {
            object o;

            if (!items.TryGetValue(id, out o))
            {
                obj = default(T);
                return false;
            }

            obj = (T)o;
            return true;
        }

        public void Store(string id, object obj)
        {
            items.AddOrUpdate(id, obj, (key, val) => val);
        }

        public void Remove(string id)
        {
            object _;
            items.TryRemove(id, out _);
        }
    }
}
