namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;    
    using ViewEngines;

    public class DefaultTemplateEngineSelector : ITemplateEngineSelector
    {
        private readonly IEnumerable<IViewEngineRegistry> viewEngines;

        public DefaultTemplateEngineSelector(IEnumerable<IViewEngineRegistry> viewEngines)
        {
            this.viewEngines = viewEngines;
        }

        public IViewEngine DefaultProcessor
        {
            get
            {
                return new StaticViewEngine(new AspNetTemplateLocator());
            }
            //TODO - AspNetTemplateLocator -> IViewLocator via constructor parameter
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