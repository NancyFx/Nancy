namespace Nancy.ViewEngines.Razor 
{
    using System.IO;
    using System.Web.Hosting;

    public class AspNetTemplateLocator : IViewLocator
    {
        public ViewLocationResult GetTemplateContents(string viewTemplate)
        {
            var path = HostingEnvironment.MapPath(viewTemplate);
            return new ViewLocationResult(path, new StreamReader(path));
        }
    }
}
