namespace Nancy.ViewEngines.Spark
{
    using System.Collections.Generic;
    using Nancy.Localization;

    public class SparkRenderContextWrapper : IRenderContext
    {
        private readonly IRenderContext innerContext;
        private readonly global::Spark.SparkViewEngine engine;

        public SparkRenderContextWrapper(IRenderContext innerContext, global::Spark.SparkViewEngine engine)
        {
            this.innerContext = innerContext;
            this.engine = engine;
        }

        public NancyContext Context
        {
            get { return this.innerContext.Context; }
        }

        public IViewCache ViewCache
        {
            get { return this.innerContext.ViewCache; }
        }

        public ITextResource TextResource
        {
            get { return this.innerContext.TextResource; }
        }

        public dynamic TextResourceFinder
        {
            get { return this.innerContext.TextResourceFinder; }
        }

        public string ParsePath(string input)
        {
            var siteRoot = this.innerContext.ParsePath("~/");
            return this.engine.ResourcePathManager.GetResourcePath(siteRoot, input);
        }

        public string HtmlEncode(string input)
        {
            return this.innerContext.HtmlEncode(input);
        }

        public ViewLocationResult LocateView(string viewName, dynamic model)
        {
            return this.innerContext.LocateView(viewName, model);
        }

        public KeyValuePair<string, string> GetCsrfToken()
        {
            return this.innerContext.GetCsrfToken();
        }
    }
}