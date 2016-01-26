namespace Nancy.Responses.Negotiation
{
    using System;
    using System.Collections.Generic;
    using Nancy.Configuration;
    using Nancy.ViewEngines;

    /// <summary>
    /// Processes the model for view requests.
    /// </summary>
    public class ViewProcessor : IResponseProcessor
    {
        private readonly IViewFactory viewFactory;
        private readonly TraceConfiguration traceConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewProcessor"/> class,
        /// with the provided <paramref name="viewFactory"/>.
        /// </summary>
        /// <param name="viewFactory">The view factory that should be used to render views.</param>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public ViewProcessor(IViewFactory viewFactory, INancyEnvironment environment)
        {
            this.viewFactory = viewFactory;
            this.traceConfiguration = environment.GetValue<TraceConfiguration>();
        }

        /// <summary>
        /// Gets a set of mappings that map a given extension (such as .json)
        /// to a media range that can be sent to the client in a vary header.
        /// </summary>
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { yield break; }
        }

        /// <summary>
        /// Determines whether the processor can handle a given content type and model.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="ProcessorMatch"/> result that determines the priority of the processor.</returns>
        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            var matchingContentType =
                requestedMediaRange.Matches("text/html");

            return matchingContentType
                ? new ProcessorMatch { ModelResult = MatchResult.DontCare, RequestedContentTypeResult = MatchResult.ExactMatch }
                : new ProcessorMatch();
        }

        /// <summary>
        /// Process the response.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="Response"/> instance.</returns>
        public Response Process(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            var viewResponse = this.viewFactory.RenderView(context.NegotiationContext.ViewName, model, GetViewLocationContext(context));

            return this.traceConfiguration.DisplayErrorTraces
                ? new MaterialisingResponse(viewResponse)
                : viewResponse;
        }

        private static ViewLocationContext GetViewLocationContext(NancyContext context)
        {
            return new ViewLocationContext
            {
                Context = context,
                ModuleName = context.NegotiationContext.ModuleName,
                ModulePath = context.NegotiationContext.ModulePath
            };
        }
    }
}