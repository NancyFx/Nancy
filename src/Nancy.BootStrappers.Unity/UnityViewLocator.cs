namespace Nancy.Bootstrappers.Unity
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.Unity;
    using ViewEngines;

    /// <summary>
    /// This class provides a workaround for Unitys lack of support for <see cref="IEnumerable{T}"/> dependencies. No additional
    /// functionality should be added to this type.
    /// </summary>
    public sealed class UnityViewLocator : ViewLocator
    {
        /// <summary>
        /// The <see cref="Type"/> of the <see cref="UnityViewLocator"/> type.
        /// </summary>
        public static Type UnityViewLocatorType = typeof(UnityViewLocator);

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityViewLocator"/> class.
        /// </summary>
        /// <param name="container">A <see cref="IUnityContainer"/> instance where dependencies can be resolved from.</param>
        public UnityViewLocator(IUnityContainer container)
            : base(container.ResolveAll<IViewSourceProvider>())
        {
        }
    }
}