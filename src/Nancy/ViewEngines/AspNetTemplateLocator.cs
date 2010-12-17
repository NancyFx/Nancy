namespace Nancy.ViewEngines 
{
    using System.Web.Hosting;

    public class AspNetTemplateLocator : IViewLocator
    {
        public string GetTemplateContents(string viewTemplate)
        {
            return HostingEnvironment.MapPath(viewTemplate);
        }
    }
}
