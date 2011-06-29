namespace Nancy.ViewEngines.NDjango
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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

        volatile TemplateManagerProvider provider;

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            if (provider == null) {
                provider = new TemplateManagerProvider().WithLoader(new TemplateLoader(renderContext));
            }
            return stream =>{

                var myname = Path.Combine(viewLocationResult.Location, viewLocationResult.Name + (string.IsNullOrEmpty(viewLocationResult.Extension) ? "" : "." + viewLocationResult.Extension));
                var templateManager = provider.GetNewManager();

                
                
                var context = new Dictionary<string, object> { { "Model", model } };
                var reader = templateManager.GetTemplate(myname).Walk(templateManager, context);

                var writer =
                    new StreamWriter(stream);

                writer.Write(reader.ReadToEnd());           
                writer.Flush();
            };
        }
    }
}