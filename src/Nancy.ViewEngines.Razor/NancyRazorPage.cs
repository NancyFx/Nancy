using Microsoft.AspNetCore.Mvc.Razor;

namespace Nancy.ViewEngines.Razor
{
    using Microsoft.AspNetCore.Mvc.Razor.Internal;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    /// <summary>
    /// Default base class for Nancy razor views
    /// </summary>
    public abstract class NancyRazorPage : RazorPage
    {
        public new dynamic ViewBag
        {
            get
            {
                var viewContext = this.ViewContext as NancyViewContext;
                return viewContext == null ? null : viewContext.RenderContext.Context.ViewBag;
            }
        }
    }

    /// <summary>
    /// Default base class for Nancy Razor views with typed models.
    /// Copied from ASP.NET MVC Core <see cref="RazorPage{TModel}"/>.
    /// </summary>
    public abstract class NancyRazorPage<TModel> : NancyRazorPage
    {
        /// <summary>
        /// Gets the Model property of the <see cref="P:Microsoft.AspNetCore.Mvc.Razor.RazorPage`1.ViewData" /> property.
        /// </summary>
        public TModel Model
        {
            get
            {
                if (this.ViewData != null)
                    return this.ViewData.Model;
                return default(TModel);
            }
        }

        /// <summary>Gets or sets the dictionary for view data.</summary>
        [RazorInject]
        public ViewDataDictionary<TModel> ViewData { get; set; }
    }
}