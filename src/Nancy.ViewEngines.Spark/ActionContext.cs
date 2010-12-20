using System.Collections.Generic;
using System.Web;

namespace Nancy.ViewEngines.Spark
{
    public class ActionContext
    {
        private readonly IDictionary<string, object> extraData;
        private readonly HttpContextBase httpContext;
        private readonly string viewPath;

        public ActionContext(HttpContextBase httpContext, string viewPath)
        {
            this.httpContext = httpContext;
            this.viewPath = viewPath;
            extraData = new Dictionary<string, object>();
        }

        public HttpContextBase HttpContext
        {
            get { return httpContext; }
        }

        public string ViewPath
        {
            get { return viewPath; }
        }

        public IDictionary<string, object> ExtraData
        {
            get { return extraData; }
        }
    }
}