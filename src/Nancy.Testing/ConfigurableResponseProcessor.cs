namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Provides a way to define an <see cref="IResponseProcessor"/> though an API.
    /// </summary>
    public class ConfigurableResponseProcessor : IResponseProcessor
    {
        private Func<MediaRange, object, NancyContext, ProcessorMatch> canProcess = (media, model, context) => new ProcessorMatch();
        private Func<MediaRange, object, NancyContext, Response> process = (media, model, context) => 200;
        private IList<Tuple<string, MediaRange>> extensionMappings = new List<Tuple<string, MediaRange>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableResponseProcessor"/> class.
        /// </summary>
        public ConfigurableResponseProcessor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableResponseProcessor"/> class,
        /// with the procided configuration.
        /// </summary>
        /// <param name="action"></param>
        public ConfigurableResponseProcessor(Action<ConfigurableResponseProcessorConfigurator> action)
        {
            var configurator =
                new ConfigurableResponseProcessorConfigurator(this);

            action.Invoke(configurator);
        }

        /// <summary>
        /// Gets a set of mappings that map a given extension (such as .json)
        /// to a media range that can be sent to the client in a vary header.
        /// </summary>
        public IEnumerable<Tuple<string, MediaRange>> ExtensionMappings
        {
            get { return this.extensionMappings; }
        }

        /// <summary>
        /// Determines whether the the processor can handle a given content type and model.
        /// </summary>
        /// <param name="requestedMediaRange">Content type requested by the client.</param>
        /// <param name="model">The model for the given media range.</param>
        /// <param name="context">The nancy context.</param>
        /// <returns>A <see cref="ProcessorMatch"/> result that determines the priority of the processor.</returns>
        public ProcessorMatch CanProcess(MediaRange requestedMediaRange, dynamic model, NancyContext context)
        {
            return this.canProcess.Invoke(requestedMediaRange, model, context);
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
            return this.process.Invoke(requestedMediaRange, model, context);
        }

        /// <summary>
        /// Provides an API for configuring a <see cref="ConfigurableResponseProcessor"/> instance.
        /// </summary>
        public class ConfigurableResponseProcessorConfigurator
        {
            private readonly ConfigurableResponseProcessor processor;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigurableResponseProcessor"/> class,
            /// for the provided <paramref name="processor"/>.
            /// </summary>
            /// <param name="processor">The <see cref="ConfigurableResponseProcessor"/> instance that will be configured.</param>
            public ConfigurableResponseProcessorConfigurator(ConfigurableResponseProcessor processor)
            {
                this.processor = processor;
            }

            /// <summary>
            /// Adds an extension mapping.
            /// </summary>
            /// <param name="extension">The extension that should be mapped.</param>
            /// <param name="range">The media range that is mapped to the extension.</param>
            /// <returns>An instance to the current <see cref="ConfigurableResponseProcessorConfigurator"/>.</returns>
            public ConfigurableResponseProcessorConfigurator Extension(string extension, MediaRange range)
            {
                this.processor.extensionMappings.Add(new Tuple<string, MediaRange>(extension, range));
                return this;
            }

            /// <summary>
            /// Adds an extension mapping.
            /// </summary>
            /// <param name="mapping">The extension mapping that should be added.</param>
            /// <returns>An instance to the current <see cref="ConfigurableResponseProcessorConfigurator"/>.</returns>
            public ConfigurableResponseProcessorConfigurator Extension(Tuple<string, MediaRange> mapping)
            {
                this.processor.extensionMappings.Add(mapping);
                return this;
            }

            /// <summary>
            /// Adds a collection of extension mappings
            /// </summary>
            /// <param name="mappings">The list of extension mappings that should be added.</param>
            /// <returns>An instance to the current <see cref="ConfigurableResponseProcessorConfigurator"/>.</returns>
            public ConfigurableResponseProcessorConfigurator Extensions(IEnumerable<Tuple<string, MediaRange>> mappings)
            {
                this.processor.extensionMappings = this.processor.extensionMappings.Concat(mappings).ToList();
                return this;
            }

            /// <summary>
            /// Set the action that should be invoked when the <see cref="IResponseProcessor.CanProcess"/> method is called.
            /// </summary>
            /// <param name="action">The action that should be performed.</param>
            /// <returns>An instance to the current <see cref="ConfigurableResponseProcessorConfigurator"/>.</returns>
            public ConfigurableResponseProcessorConfigurator CanProcess(Func<MediaRange, object, NancyContext, ProcessorMatch> action)
            {
                this.processor.canProcess = action;
                return this;
            }

            /// <summary>
            /// Set the action that should be invoked when the <see cref="IResponseProcessor.Process"/> method is called.
            /// </summary>
            /// <param name="action">The action that should be performed.</param>
            /// <returns>An instance to the current <see cref="ConfigurableResponseProcessorConfigurator"/>.</returns>
            public ConfigurableResponseProcessorConfigurator Process(Func<MediaRange, object, NancyContext, Response> action)
            {
                this.processor.process = action;
                return this;
            }
        }
    }
}