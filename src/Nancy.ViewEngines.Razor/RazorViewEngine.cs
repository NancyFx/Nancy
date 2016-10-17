namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.ObjectPool;
    using Nancy.Configuration;
    using Nancy.Responses;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using Microsoft.AspNetCore.Mvc.Razor.Compilation;
    using Microsoft.AspNetCore.Mvc.Razor.Internal;
    using Microsoft.Extensions.FileProviders;
    using Microsoft.Extensions.Primitives;

    /// <summary>
    /// View engine for rendering razor views.
    /// </summary>
    public class RazorViewEngine : Nancy.ViewEngines.IViewEngine
    {
        public static readonly string[] SupportedExtensions = { "cshtml" };

        private readonly IRazorConfiguration razorConfiguration;

        private readonly IAssemblyCatalog assemblyCatalog;

        private readonly IViewLocationProvider locationProvider;

        private readonly INancyEnvironment environment;

        private IServiceProvider razorServices;

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return SupportedExtensions; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewEngine"/> class.
        /// </summary>
        public RazorViewEngine(IRazorConfiguration razorConfiguration, IAssemblyCatalog assemblyCatalog, IViewLocationProvider locationProvider, INancyEnvironment environment)
        {
            this.razorConfiguration = razorConfiguration;
            this.assemblyCatalog = assemblyCatalog;
            this.locationProvider = locationProvider;
            this.environment = environment;
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            var mvcBuilder = serviceCollection
                .AddMvcCore()
                .AddRazorViewEngine();
            mvcBuilder.PartManager.FeatureProviders.Add(new MetadataReferenceFeatureProvider(this.razorConfiguration,
                this.assemblyCatalog));
            serviceCollection.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });
            
            serviceCollection
                .AddSingleton<ILoggerFactory, SilentLoggerFactory>()
                .AddSingleton<DiagnosticSource, SilentDiagnosticSource>()
                .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>()
                .AddSingleton(this.razorConfiguration)
                .AddSingleton<IHostingEnvironment, HostingEnvironment>()
                .AddScoped<IMvcRazorHost, NancyRazorHost>()
                .AddScoped<RazorViewRenderer>();

            this.razorServices = serviceCollection.BuildServiceProvider();
        }

        public Response RenderView(ViewLocationResult viewLocation, dynamic model, IRenderContext renderContext)
        {
            var scopeFactory = this.razorServices.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                // Set the file provider in the hosting environment with the current model and render context before filling other services
                var hostingEnvironment = scope.ServiceProvider.GetRequiredService<IHostingEnvironment>();
                hostingEnvironment.WebRootFileProvider = new ViewLocationFileProvider(this.locationProvider, renderContext, model);

                // Get the view renderer and render the view
                var viewRenderer = scope.ServiceProvider.GetRequiredService<RazorViewRenderer>();
                var response = new HtmlResponse
                {
                    Contents = stream => viewRenderer.RenderView(viewLocation, model, renderContext, this.environment, stream)
                };
                return response;
            }
        }
    }
}
