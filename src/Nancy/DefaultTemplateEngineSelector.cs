namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;    
    using ViewEngines;

    public class DefaultTemplateEngineSelector : ITemplateEngineSelector
    {
        private readonly IEnumerable<IViewEngineRegistry> viewEngines;

        public DefaultTemplateEngineSelector(IEnumerable<IViewEngineRegistry> viewEngines)
        {
            this.viewEngines = viewEngines;
        }

        public Func<string, TModel, Action<Stream>> DefaultProcessor<TModel>()
        {
            return (path, model) =>
                       {
                           //TODO - AspNetTemplateLocator -> IViewLocator via constructor parameter
                           var staticViewEngine = new StaticViewEngine(new AspNetTemplateLocator());
                           return (Action<Stream>) (stream =>
                                                        {
                                                            var result = staticViewEngine.RenderView(path, model);
                                                            result.Execute(stream);
                                                        });
                       };
        }

        public Func<string, TModel, Action<Stream>> GetTemplateProcessor<TModel>(string extension)
        {
            var viewEngineRegistry = viewEngines.SingleOrDefault(registry => registry.Extension == extension);
            if (viewEngineRegistry == null)
            {
                return null;
            }
            return (name, model) => viewEngineRegistry.Execute(name, model);
        }
    }
}