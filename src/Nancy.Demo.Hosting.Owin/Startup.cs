using System.Reflection;
using Nancy.Hosting.Owin.Extensions;
using Owin;

namespace Nancy.Demo.Hosting.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder builder)
        {
            // ensure the bootstrapper sees some assemblies
            Assembly.Load("Nancy.ViewEngines.Spark");

            // wire up middleware and framework
            builder.RunNancy();
        }
    }
}