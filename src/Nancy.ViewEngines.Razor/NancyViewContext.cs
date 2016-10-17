namespace Nancy.ViewEngines.Razor
{
    using System.IO;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public class NancyViewContext : ViewContext
    {
        private readonly IRenderContext renderContext;

        public NancyViewContext(IRenderContext renderContext, ActionContext actionContext, IView view, ViewDataDictionary viewData, 
            ITempDataDictionary tempData, TextWriter writer, HtmlHelperOptions htmlHelperOptions) 
            : base(actionContext, view, viewData, tempData, writer, htmlHelperOptions)
        {
            this.renderContext = renderContext;
        }

        public IRenderContext RenderContext
        {
            get { return this.renderContext; }
        }
    }
}