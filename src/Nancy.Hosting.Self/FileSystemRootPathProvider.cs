namespace Nancy.Hosting.Self
{
#if DNX451
    using Microsoft.Framework.Runtime;
    using Microsoft.Framework.Runtime.Infrastructure;
#endif

    public class FileSystemRootPathProvider : IRootPathProvider
    {
        private readonly IApplicationEnvironment env;

        public FileSystemRootPathProvider()
        {
#if DNX451
            this.env = CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(IApplicationEnvironment)) as IApplicationEnvironment;
#endif
        }

        public string GetRootPath()
        {
#if DNX451

            return this.env.ApplicationBasePath;

#else
            var assembly = Assembly.GetEntryAssembly();

            return assembly != null ?
                Path.GetDirectoryName(assembly.Location) :
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
#endif
        }
    }
}
