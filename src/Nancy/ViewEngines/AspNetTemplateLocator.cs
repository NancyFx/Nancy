namespace Nancy.ViewEngines 
{
    using System.Web.Hosting;

    public class AspNetTemplateLocator : IViewLocator
    {
        public string GetFullPath(string viewTemplate)
        {
            return HostingEnvironment.MapPath(viewTemplate);
        }
    }
}
