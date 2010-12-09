using System.IO;
using System.Web.Hosting;

namespace Nancy.ViewEngines.Razor {
    public class AspNetTemplateLocator : IViewLocator {
        public ViewLocationResult GetTemplateContents(string viewTemplate) {
            string path = HostingEnvironment.MapPath(viewTemplate);
            return new ViewLocationResult(path, new StreamReader(path));
        }
    }
}
