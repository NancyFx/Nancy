namespace Nancy.ViewEngines 
{
    using System.IO;
    using System.Web.Hosting;

    public class AspNetTemplateLocator : IViewLocator
    {
        public ViewLocationResult GetTemplateContents(string viewTemplate)
        {
            var path = HostingEnvironment.MapPath(viewTemplate);
			using (var fs = File.OpenRead(path))
			{
				var stream = new MemoryStream();
				fs.CopyTo(stream);
			    stream.Position = 0;
				return new ViewLocationResult(path, new StreamReader(stream));
			}
        }
    }
}
