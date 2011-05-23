namespace Nancy.ViewEngines
{
    using System;

    public interface IRenderContext
    {
        string HtmlEncode(string input);

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

    public string HtmlEncode(string input)
    {
        return Helpers.HttpUtility.HtmlEncode(input);
    }

    public ViewLocationResult LocateView(string viewName, dynamic model)
    {
        return this.viewResolver.GetViewLocation(viewName, model, this.viewLocationContext);
    }
}
}