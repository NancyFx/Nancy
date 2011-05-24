namespace Nancy.ViewEngines
{
    using System;

    public interface IRenderContext
    {
        string HtmlEncode(string input);

        IViewCache ViewCache { get; }

        ViewLocationResult LocateView(string viewName, dynamic model);
    }

    public interface IRenderContextFactory
    {
        IRenderContext GetRenderContext(ViewLocationContext viewLocationContext);
    }

    public class DefaultRenderContextFactory : IRenderContextFactory
    {
        private readonly IViewCache viewCache;
        private readonly IViewResolver viewResolver;

        public DefaultRenderContextFactory(IViewCache viewCache, IViewResolver viewResolver)
        {
            this.viewCache = viewCache;
            this.viewResolver = viewResolver;
        }

        public IRenderContext GetRenderContext(ViewLocationContext viewLocationContext)
        {
            return new DefaultRenderContext(this.viewResolver, this.viewCache, viewLocationContext);
        }
    }

    public class DefaultRenderContext : IRenderContext
    {
        private readonly IViewResolver viewResolver;
        private readonly IViewCache viewCache;
        private readonly ViewLocationContext viewLocationContext;

        public DefaultRenderContext(IViewResolver viewResolver, IViewCache viewCache, ViewLocationContext viewLocationContext)
        {
            this.viewResolver = viewResolver;
            this.viewCache = viewCache;
            this.viewLocationContext = viewLocationContext;
        }

        public string HtmlEncode(string input)
        {
            return Helpers.HttpUtility.HtmlEncode(input);
        }

        public IViewCache ViewCache
        {
            get { return this.viewCache; }
        }

        public ViewLocationResult LocateView(string viewName, dynamic model)
        {
            return this.viewResolver.GetViewLocation(viewName, model, this.viewLocationContext);
        }
    }
}