namespace Nancy.Demo.Owin
{
    using System.Web.Hosting;

    public class AspNetRootSourceProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return HostingEnvironment.MapPath("~/");
        }
    }
}