namespace Nancy.Bootstrappers.Unity
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// This class provides a workaround for Unitys lack of support for <see cref="IEnumerable{T}"/> dependencies. No additional
    /// functionality should be added to this type.
    /// </summary>
    public class UnityEnumerableShim<T> : IEnumerable<T>
    {
        private readonly IUnityContainer existingContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityEnumerableShim{T}"/> class.
        /// </summary>
        /// <param name="existingContainer">A <see cref="IUnityContainer"/> instance where dependencies can be resolved from.</param>
        public UnityEnumerableShim(IUnityContainer existingContainer)
        {
            this.existingContainer = existingContainer;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return existingContainer.ResolveAll<T>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}