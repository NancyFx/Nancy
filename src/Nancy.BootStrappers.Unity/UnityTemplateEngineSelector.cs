using Microsoft.Practices.Unity;
using Nancy.ViewEngines;

namespace Nancy.Bootstrappers.Unity
{
    public class UnityTemplateEngineSelector : DefaultTemplateEngineSelector
    {
        public UnityTemplateEngineSelector(IUnityContainer container)
            : base(container.ResolveAll<IViewEngineRegistry>())
        {
        }
    }
}
