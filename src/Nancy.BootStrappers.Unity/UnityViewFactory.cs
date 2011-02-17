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
    public sealed class UnityViewFactory : DefaultViewFactory
    {
        /// <summary>
        /// The <see cref="Type"/> of the <see cref="UnityViewFactory"/> type.
        /// </summary>
        public static Type UnityViewFactoryType = typeof(UnityViewFactory);

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityViewFactory"/> class.
        /// </summary>
        /// <param name="container">A <see cref="IUnityContainer"/> instance where dependencies can be resolved from.</param>
        public UnityViewFactory(IUnityContainer container)
            : base(container.Resolve<IViewLocator>(), container.ResolveAll<IViewEngineEx>())
        {
        }
    }
}