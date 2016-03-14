namespace Nancy
{
    /// <summary>
    /// A cache for empty arrays.
    /// </summary>
    public class ArrayCache
    {
        /// <summary>
        /// Gets a cached, empty array of the specified type.
        /// </summary>
        /// <typeparam name="T">the type of array to get.</typeparam>
        public static T[] Empty<T>()
        {
            return EmptyArray<T>.Value;
        }

        private static class EmptyArray<T>
        {
            public static readonly T[] Value = new T[0];
        }
    }
}
