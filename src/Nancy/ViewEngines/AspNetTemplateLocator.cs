namespace Nancy.ViewEngines 
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web.Hosting;

    public class AspNetTemplateLocator : IViewLocator
    {
        public ViewLocationResult GetViewLocation(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            var path = HostingEnvironment.MapPath(viewName);
			using (var fs = File.OpenRead(path))
			{
				var stream = new MemoryStream();
				fs.CopyTo(stream);
			    stream.Position = 0;
				return new ViewLocationResult(path, string.Empty, new StreamReader(stream));
			}
        }
    }
}
