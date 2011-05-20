namespace Nancy.ViewEngines
{
    using System;

    public interface IRenderContext
    {
        ViewLocationResult LocateView(string viewName, dynamic model);
    }

public class DefaultRenderContext : IRenderContext
{
    private readonly IViewResolver viewResolver;
    private readonly ViewLocationContext viewLocationContext;

    public DefaultRenderContext(IViewResolver viewResolver, ViewLocationContext viewLocationContext)
    {
        this.viewResolver = viewResolver;
        this.viewLocationContext = viewLocationContext;
    }

    public ViewLocationResult LocateView(string viewName, dynamic model)
    {
        return this.viewResolver.GetViewLocation(viewName, model, this.viewLocationContext);
    }
}
}