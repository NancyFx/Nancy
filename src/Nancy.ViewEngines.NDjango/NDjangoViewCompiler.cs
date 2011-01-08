namespace Nancy.ViewEngines.NDjango
{
    using global::NDjango;
    using global::NDjango.Interfaces;

    public class NDjangoViewCompiler : IViewCompiler
    {
        public IView GetCompiledView<TModel>(IViewLocationResult viewLocationResult)
        {
            var templateManagerProvider = new TemplateManagerProvider()
                .WithLoader(new TemplateLoader(viewLocationResult.Contents));

            var templateManager = templateManagerProvider.GetNewManager();

            ITemplate template = templateManager.GetTemplate(string.Empty);

            return new NDjangoView(template, templateManager);
        }
    }
}