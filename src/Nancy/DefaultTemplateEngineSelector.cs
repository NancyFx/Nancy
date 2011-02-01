namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using ViewEngines;

    public class DefaultTemplateEngineSelector : ITemplateEngineSelector
    {
        private readonly IEnumerable<IViewEngineRegistry> viewEngines;
        private readonly StaticViewEngine defaultProcessor;

        public DefaultTemplateEngineSelector(IEnumerable<IViewEngineRegistry> viewEngines, IViewLocator viewLocator)
        {
            this.viewEngines = viewEngines;
            this.defaultProcessor = new StaticViewEngine(viewLocator);
        }

        public IViewEngine DefaultProcessor
        {
            get { return defaultProcessor; }
        }

        public IViewEngine GetTemplateProcessor(string extension)
        {
            var engineRegistry = viewEngines.SingleOrDefault(e => e.Extension.Equals(extension, StringComparison.CurrentCultureIgnoreCase));
            if (engineRegistry == null)
            {
                return null;
            }
            return engineRegistry.ViewEngine;
        }
    }
}
