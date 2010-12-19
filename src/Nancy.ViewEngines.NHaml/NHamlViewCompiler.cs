namespace Nancy.ViewEngines.NHaml
{
    using System.Collections.Generic;
    using System.IO;
    using global::NHaml;
    using global::NHaml.TemplateResolution;

    public class NHamlViewCompiler : IViewCompiler
    {
        public IView GetCompiledView<TModel>(TextReader textReader)
        {
            var templateEngine = new TemplateEngine();

            var viewSource = new ViewSource(textReader);

            var compiledTemplate = templateEngine.Compile(new List<IViewSource> {viewSource}, typeof (NHamlView<TModel>));
            return (NHamlView<TModel>)compiledTemplate.CreateInstance();
        }
    }
}