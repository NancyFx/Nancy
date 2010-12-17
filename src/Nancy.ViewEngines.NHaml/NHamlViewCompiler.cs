namespace Nancy.ViewEngines.NHaml
{
    using global::NHaml;

    public class NHamlViewCompiler : IViewCompiler
    {
        public IView GetCompiledView<TModel>(string fullPath)
        {
            var templateEngine = new TemplateEngine();

            var compiledTemplate = templateEngine.Compile(fullPath, typeof(NHamlView<TModel>));
            return (NHamlView<TModel>)compiledTemplate.CreateInstance();
        }
    }
}