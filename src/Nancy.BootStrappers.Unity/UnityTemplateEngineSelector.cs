using Microsoft.Practices.Unity;
using Nancy.ViewEngines;

namespace Nancy.BootStrappers.Unity
{
    public class UnityTemplateEngineSelector : DefaultTemplateEngineSelector
    {
        public UnityTemplateEngineSelector(IUnityContainer container)
            : base(container.ResolveAll<IViewEngineRegistry>())
        {
        }
    }
}
