using System.Collections.Generic;
using Nancy.Localization;

namespace Nancy.ViewEngines.Spark
{
    public class SparkRenderContextWrapper : IRenderContext
    {
        private readonly IRenderContext _innerContext;
        private readonly global::Spark.SparkViewEngine _engine;

        public SparkRenderContextWrapper(IRenderContext innerContext, global::Spark.SparkViewEngine engine)
        {
            _innerContext = innerContext;
            _engine = engine;
        }

        public NancyContext Context
        {
            get { return _innerContext.Context; }
        }

        public IViewCache ViewCache
        {
            get { return _innerContext.ViewCache; }
        }

        public ITextResource TextResource
        {
            get { return _innerContext.TextResource; }
        }

        public dynamic TextResourceFinder
        {
            get { return _innerContext.TextResourceFinder; }
        }

        public string ParsePath(string input)
        {
            string siteRoot = _innerContext.ParsePath("~/");
            return _engine.ResourcePathManager.GetResourcePath(siteRoot, input);
        }

        public string HtmlEncode(string input)
        {
            return _innerContext.HtmlEncode(input);
        }

        public ViewLocationResult LocateView(string viewName, dynamic model)
        {
            return _innerContext.LocateView(viewName, model);
        }

        public KeyValuePair<string, string> GetCsrfToken()
        {
            return _innerContext.GetCsrfToken();
        }
    }
}