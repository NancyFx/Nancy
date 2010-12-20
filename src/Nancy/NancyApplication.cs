namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ViewEngines;

    public class NancyApplication : INancyApplication
    {
        private readonly IDictionary<string, Func<string, object, Action<Stream>>> templateProcessors;

        public NancyApplication()
        {
            this.templateProcessors = LoadTemplates();
        }

        public Func<string, object, Action<Stream>> GetTemplateProcessor(string extension)
        {
            return this.templateProcessors.ContainsKey(extension) ? this.templateProcessors[extension] : null;
        }

        public Func<string, object, Action<Stream>> DefaultProcessor
        {
            get { return (path, model) => StaticViewEngineExtension.Static(null, path); }
        }

        private static IDictionary<string, Func<string, object, Action<Stream>>> LoadTemplates()
        {
            var registries = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where !type.IsAbstract && typeof (IViewEngineRegistry).IsAssignableFrom(type)
                             select type;

            var templates = new Dictionary<string, Func<string, object, Action<Stream>>>(registries.Count(), StringComparer.CurrentCultureIgnoreCase);
            foreach (var type in registries)
            {
                var registry = (IViewEngineRegistry) Activator.CreateInstance(type);
                templates.Add(registry.Extension, registry.Executor);
            }
            return templates;
        }
    }
}