namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;    
    using Extensions;
    using ViewEngines;

    public class DefaultTemplateEngineSelector : ITemplateEngineSelector
    {
        private readonly IDictionary<string, Func<string, object, Action<Stream>>> templateProcessors;

        public DefaultTemplateEngineSelector(IEnumerable<IViewEngineRegistry> viewEngines)
        {
            this.templateProcessors = LoadTemplates(viewEngines);
        }

        public Func<string, object, Action<Stream>> GetTemplateProcessor(string extension)
        {
            return this.templateProcessors.ContainsKey(extension) ? this.templateProcessors[extension] : null;
        }

        public Func<string, object, Action<Stream>> DefaultProcessor
        {
            get { return (path, model) => StaticViewEngineExtension.Static(null, path); }
        }

        private static IDictionary<string, Func<string, object, Action<Stream>>> LoadTemplates(IEnumerable<IViewEngineRegistry> viewEngines)
        {
            var templates = new Dictionary<string, Func<string, object, Action<Stream>>>(viewEngines.Count(), StringComparer.CurrentCultureIgnoreCase);
            foreach (var viewEngine in viewEngines)
            {
                templates.Add(viewEngine.Extension, viewEngine.Executor);
            }
            return templates;
        }
    }
}