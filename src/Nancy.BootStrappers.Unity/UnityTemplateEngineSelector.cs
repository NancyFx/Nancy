namespace Nancy.Bootstrappers.Unity
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.Unity;
    using Nancy.ViewEngines;

    /// <summary>
    /// This class provides a workaround for Unitys lack of support for <see cref="IEnumerable{T}"/> dependencies. No additional
    /// functionality should be added to this type.
    /// </summary>
    public sealed class UnityTemplateEngineSelector : DefaultTemplateEngineSelector
    {
        public static Type UnityTemplateEngineSelectorType = typeof(UnityTemplateEngineSelector);

        public UnityTemplateEngineSelector(IUnityContainer container)
            : base(container.ResolveAll<IViewEngineRegistry>(), container.Resolve<IViewLocator>())
        {
        }
    }
}