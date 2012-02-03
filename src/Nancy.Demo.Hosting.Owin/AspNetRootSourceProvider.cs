using System;

namespace Nancy.Demo.Hosting.Owin
{
    using System.Web.Hosting;

    public class AspNetRootSourceProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return HostingEnvironment.IsHosted 
                ? HostingEnvironment.MapPath("~/") 
                : Environment.CurrentDirectory;
        }
    }
}