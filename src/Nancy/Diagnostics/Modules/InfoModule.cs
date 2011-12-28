namespace Nancy.Diagnostics.Modules
{
    using System;
    using System.Dynamic;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.ViewEngines;

    public class InfoModule : DiagnosticModule
    {
        public InfoModule(IRootPathProvider rootPathProvider)
            : base("/info")
        {
            Get["/"] = _ => View["Info"];

            Get["/data"] = _ =>
            {
                dynamic data = new ExpandoObject();

                data.Nancy = new ExpandoObject();
                data.Nancy.Version = string.Format("v{0}", this.GetType().Assembly.GetName().Version.ToString());
                data.Nancy.CachesDisabled = StaticConfiguration.DisableCaches;
                data.Nancy.TracesDisabled = StaticConfiguration.DisableErrorTraces;
                data.Nancy.CaseSensitivity = StaticConfiguration.CaseSensitive ? "Sensitive" : "Insensitive";
                data.Nancy.RootPath = rootPathProvider.GetRootPath();
                data.Nancy.Hosting = this.GetHosting();
                data.Nancy.BootstrapperContainer = this.GetBootstrapperContainer();
                data.Nancy.LocatedBootstrapper = NancyBootstrapperLocator.Bootstrapper.GetType().ToString();
                data.Nancy.LoadedViewEngines = GetViewEngines();

                return Response.AsJson((object)data);
            };
        }

        private string[] GetViewEngines()
        {
            var engines = AppDomainAssemblyTypeScanner.TypesOf<IViewEngine>();

            return engines.Select(engine => engine.Name.Split(new [] { "ViewEngine" }, StringSplitOptions.None)[0]).ToArray();
        }

        private string GetBootstrapperContainer()
        {
            var name = AppDomain.CurrentDomain.GetAssemblies()
                                                  .Select(asm => asm.GetName())
                                                  .FirstOrDefault(asmName => asmName.Name != null && asmName.Name.StartsWith("Nancy.Bootstrappers."));

            if (name == null)
            {
                return "TinyIoC";
            }

            return string.Format("{0} (v{1})", name.Name.Split('.').Last(), name.Version);
        }

        private string GetHosting()
        {
            var name = AppDomain.CurrentDomain.GetAssemblies()
                                                  .Select(asm => asm.GetName())
                                                  .FirstOrDefault(asmName => asmName.Name != null && asmName.Name.StartsWith("Nancy.Hosting."));

            if (name == null)
            {
                return "Unknown";
            }

            return string.Format("{0} (v{1})", name.Name.Split('.').Last(), name.Version);
        }
    }
}