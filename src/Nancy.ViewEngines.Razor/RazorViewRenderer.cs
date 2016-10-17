namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Encodings.Web;
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Razor.Compilation;
    using Microsoft.AspNetCore.Mvc.Razor.Internal;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Emit;
    using Microsoft.Extensions.Primitives;
    using Nancy.Configuration;
    using Nancy.Helpers;
    using Nancy.ViewEngines;
    using Nancy.ViewEngines.Razor;

    public class RazorViewRenderer
    {
        private readonly IServiceProvider serviceProvider;

        private readonly IRazorViewEngine viewEngine;

        private readonly IRazorPageActivator pageActivator;

        private readonly HtmlEncoder htmlEncoder;

        private readonly IRazorPageFactoryProvider pageFactoryProvider;

        private readonly IRazorCompilationService razorCompilationService;

        private readonly ITempDataProvider tempDataProvider;

        public RazorViewRenderer(IServiceProvider serviceProvider, IRazorViewEngine viewEngine, 
            IRazorPageActivator pageActivator,
            HtmlEncoder htmlEncoder, IRazorPageFactoryProvider pageFactoryProvider,
            IRazorCompilationService razorCompilationService,
            ITempDataProvider tempDataProvider)
        {
            this.serviceProvider = serviceProvider;
            this.viewEngine = viewEngine;
            this.pageActivator = pageActivator;
            this.htmlEncoder = htmlEncoder;
            this.pageFactoryProvider = pageFactoryProvider;
            this.razorCompilationService = razorCompilationService;
            this.tempDataProvider = tempDataProvider;
        }

        public void RenderView(ViewLocationResult viewLocation, dynamic model, IRenderContext renderContext, INancyEnvironment environment, Stream stream)
        {
            // Get and render the view
            var view = this.GetViewFromViewLocation(viewLocation, environment);
            var output = new StreamWriter(stream);
            var viewContext = GetViewContext(view, model, output, renderContext);
            try
            {
                view.RenderAsync(viewContext).GetAwaiter().GetResult();
            }
            catch (InvalidOperationException ex)
            {
                // Throw a specialized exception for partial view location problems
                if (ex.Message.StartsWith("The partial view"))
                {
                    throw new ViewNotFoundException(ex.Message);
                }
                throw;
            }
            output.Flush();
        }

        private ViewContext GetViewContext(IView view, dynamic model, TextWriter output, IRenderContext renderContext)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = this.serviceProvider
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), actionContext.ModelState)
            {
                Model = model
            };
            var tempData = new TempDataDictionary(actionContext.HttpContext, this.tempDataProvider);
            var viewContext = new NancyViewContext(renderContext,
                actionContext, view, viewData, tempData, output, new HtmlHelperOptions());
            return viewContext;
        }

        /// <summary>
        /// Gets the view for a located view location.
        /// </summary>
        private IView GetViewFromViewLocation(ViewLocationResult viewLocation, INancyEnvironment environment)
        {
            var viewStartLocations = ViewHierarchyUtility.GetViewStartLocations(viewLocation.Location);
            var viewStartPages = viewStartLocations
                .Select(this.pageFactoryProvider.CreateFactory)
                .Where(x => x.Success)
                .Select(x => x.RazorPageFactory())
                .Reverse()
                .ToList();
            var page = this.GetPageFromViewLocation(viewLocation, environment);
            var activator = page is NancyRazorErrorView ? new EmptyPageActivator() : this.pageActivator;
            return new RazorView(this.viewEngine, activator, viewStartPages, page, this.htmlEncoder);
        }

        /// <summary>
        /// Gets the Razor page for a view location. This is roughly modeled on
        /// DefaultRazorPageFactory and CompilerCache.
        /// </summary>
        private IRazorPage GetPageFromViewLocation(ViewLocationResult viewLocation, INancyEnvironment environment)
        {
            // Get the file info by combining the view location with info found at the document's original location (if any)
            string relativePath = string.Format("/{0}/{1}", viewLocation.Location, viewLocation.Name);
            var fileInfo = new ViewLocationFileInfo(viewLocation);
            var relativeFileInfo = new RelativeFileInfo(fileInfo, relativePath);

            // Create the compilation
            var compilationResult = this.razorCompilationService.Compile(relativeFileInfo);

            // Check for errors
            if (compilationResult.CompilationFailures != null)
            {
                var traceConfiguration = environment.GetValue<TraceConfiguration>();
                return new NancyRazorErrorView(BuildErrorMessage(compilationResult.CompilationFailures.First(), viewLocation), traceConfiguration);
            }
            compilationResult.EnsureSuccessful();

            // Create and return the page
            // We're not actually using the cache, but the CompilerCacheResult ctor contains the logic to create the page factory
            var compilerCacheResult = new CompilerCacheResult(relativePath, compilationResult, new IChangeToken[] { });
            return compilerCacheResult.PageFactory();
        }

        private static string BuildErrorMessage(CompilationFailure failure, ViewLocationResult viewLocationResult)
        {
            var fullTemplateName = viewLocationResult.Location + "/" + viewLocationResult.Name + "." + viewLocationResult.Extension;
            var errorMessages = failure.Messages.Select(error => string.Format(
                "<li>Line: {0} Column: {1} - {2}</li>",
                error.StartLine,
                error.StartColumn,
                error.Message)).ToArray();

            var errorDetails = string.Format(
                "Template: <strong>{0}</strong><br/><br/>Errors:<ul>{1}</ul>",
                fullTemplateName,
                string.Join(Environment.NewLine, errorMessages));

            return errorDetails;
        }

        public class EmptyPageActivator : IRazorPageActivator
        {
            public void Activate(IRazorPage page, ViewContext context)
            {
            }
        }
    }
}