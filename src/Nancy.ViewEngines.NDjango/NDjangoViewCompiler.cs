namespace Nancy.ViewEngines.NDjango
{
    using System.IO;
    using global::NDjango;
    using global::NDjango.Interfaces;

    public class NDjangoViewCompiler : IViewCompiler
    {
        public IView GetCompiledView<TModel>(TextReader textReader)
        {
            var templateManagerProvider = new TemplateManagerProvider()
                .WithLoader(new TemplateLoader(textReader));

            var templateManager = templateManagerProvider.GetNewManager();

            ITemplate template = templateManager.GetTemplate(string.Empty);

            return new NDjangoView(template, templateManager);
        }
    }
}