namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using global::NDjango;
    using global::NDjango.Interfaces;

    public class NDjangoViewCompiler : IViewCompiler, IViewEngineEx
    {
        public IView GetCompiledView<TModel>(TextReader textReader)
        {
            var templateManagerProvider = new TemplateManagerProvider()
                .WithLoader(new TemplateLoader(textReader));

            var templateManager = templateManagerProvider.GetNewManager();

            ITemplate template = templateManager.GetTemplate(string.Empty);

            return new NDjangoView(template, templateManager);
        }

        public IEnumerable<string> Extensions
        {
            get { yield return "django"; }
        }

        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model)
        {
            return stream =>
            {
                var templateManagerProvider = 
                    new TemplateManagerProvider()
                    .WithLoader(new TemplateLoader(viewLocationResult.Contents));

                var templateManager = 
                    templateManagerProvider.GetNewManager();

                var template = 
                    templateManager.GetTemplate(string.Empty);

                var context = new Dictionary<string, object> { { "Model", model } };
                var reader = template.Walk(templateManager, context);

                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(reader.ReadToEnd());           
                }
            };
        }
    }
}