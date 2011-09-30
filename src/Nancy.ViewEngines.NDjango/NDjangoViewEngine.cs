namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Responses;
    using global::NDjango;

    /// <summary>
    /// View engine for rendering django views.
    /// </summary>
    public class NDjangoViewEngine : IViewEngine
    {
        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { yield return "django"; }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        private Object UnwrapDictionary(Object o)
        {
            var dict = o as DynamicDictionary;
            if (dict != null)
            {
                return new DictionaryWrapper(dict);
            }
            return o;
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A response</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return new HtmlResponse(contents: stream =>
            {
                var provider = new TemplateManagerProvider().WithLoader(new TemplateLoader(renderContext, viewLocationResult));

                var templateManager = provider.GetNewManager();
                
                var context = new Dictionary<string, object> { { "Model", UnwrapDictionary(model) } };
                
                var reader = templateManager.GetTemplate(viewLocationResult.Location).Walk(templateManager, context);

                var writer = new StreamWriter(stream);

                writer.Write(reader.ReadToEnd());           
                writer.Flush();
            });
        }
    }
}