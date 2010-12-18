namespace Nancy.ViewEngines.NDjango
{
    using global::NDjango;
    using global::NDjango.Interfaces;

    public class NDjangoViewCompiler : IViewCompiler
    {
        public IView GetCompiledView<TModel>(string fullPath)
        {
            var templateManagerProvider = new TemplateManagerProvider();
            var templateManager = templateManagerProvider.GetNewManager();

            ITemplate template = templateManager.GetTemplate(fullPath);

            return new NDjangoView(template, templateManager);
        }
    }
}