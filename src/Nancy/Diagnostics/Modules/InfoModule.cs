namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Reflection;
    using Nancy.Bootstrapper;
    using Nancy.Configuration;
    using Nancy.ViewEngines;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Nancy.Diagnostics.DiagnosticModule" />
    public class InfoModule : DiagnosticModule
    {
        private readonly ITypeCatalog typeCatalog;
        private readonly IAssemblyCatalog assemblyCatalog;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoModule"/> class.
        /// </summary>
        /// <param name="rootPathProvider">The root path provider.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="typeCatalog">The type catalog.</param>
        /// <param name="assemblyCatalog">The assembly catalog.</param>
        public InfoModule(IRootPathProvider rootPathProvider, NancyInternalConfiguration configuration, INancyEnvironment environment, ITypeCatalog typeCatalog, IAssemblyCatalog assemblyCatalog)
            : base("/info")
        {
            this.typeCatalog = typeCatalog;
            this.assemblyCatalog = assemblyCatalog;

            Get("/", _ =>
            {
                return View["Info"];
            });

            Get("/data", _ =>
            {
                dynamic data = new ExpandoObject();

                data.Nancy = new ExpandoObject();
                data.Nancy.Version = string.Format("v{0}", this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString());
                data.Nancy.TracesDisabled = !environment.GetValue<TraceConfiguration>().DisplayErrorTraces;
                data.Nancy.CaseSensitivity = StaticConfiguration.CaseSensitive ? "Sensitive" : "Insensitive";
                data.Nancy.RootPath = rootPathProvider.GetRootPath();
                data.Nancy.Hosting = GetHosting();
                data.Nancy.BootstrapperContainer = GetBootstrapperContainer();
                data.Nancy.LocatedBootstrapper = NancyBootstrapperLocator.Bootstrapper.GetType().ToString();
                data.Nancy.LoadedViewEngines = GetViewEngines();

                data.Configuration = new Dictionary<string, object>();
                foreach (var propertyInfo in configuration.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    var value =
                        propertyInfo.GetValue(configuration, null);

                    data.Configuration[propertyInfo.Name] = (!typeof(IEnumerable).IsAssignableFrom(value.GetType())) ?
                        new[] { value.ToString() } :
                        ((IEnumerable<object>)value).Select(x => x.ToString());
                }

                return this.Response.AsJson((object)data);
            });
        }

        private string[] GetViewEngines()
        {
            var engines = this.typeCatalog.GetTypesAssignableTo<IViewEngine>();

            return engines
                .Select(engine => engine.Name.Split(new [] { "ViewEngine" }, StringSplitOptions.None)[0])
                .ToArray();
        }

        private string GetBootstrapperContainer()
        {
            var name = this.assemblyCatalog
                .GetAssemblies()
                .Select(asm => asm.GetName())
                .FirstOrDefault(asmName => asmName.Name != null && asmName.Name.StartsWith("Nancy.Bootstrappers."));

            return (name == null) ?
                "TinyIoC" :
                string.Format("{0} (v{1})", name.Name.Split('.').Last(), name.Version);
        }

        private string GetHosting()
        {
            var name = this.assemblyCatalog
                .GetAssemblies()
                .Select(asm => asm.GetName())
                .FirstOrDefault(asmName => asmName.Name != null && asmName.Name.StartsWith("Nancy.Hosting."));

            return (name == null) ?
                "Unknown" :
                string.Format("{0} (v{1})", name.Name.Split('.').Last(), name.Version);
        }
    }
}