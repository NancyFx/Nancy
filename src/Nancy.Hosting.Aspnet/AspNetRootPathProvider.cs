namespace Nancy.Hosting.Aspnet
{
    using System.Web.Hosting;

    public class AspNetRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return HostingEnvironment.MapPath("~/");
        }
    }
}
