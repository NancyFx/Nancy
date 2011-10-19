namespace Nancy
{
    using System;
    using Bootstrapper;

    /// <summary>
    /// Defines the functionality of an engine that can handle Nancy <see cref="Request"/>s.
    /// </summary>
    public interface INancyEngine
    {
        /// <summary>
        /// Factory for creating an <see cref="IPipelines"/> instance for a incoming request.
        /// </summary>
        /// <value>An <see cref="IPipelines"/> instance.</value>
        Func<NancyContext, IPipelines> RequestPipelinesFactory { get; set; }
            
        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>A <see cref="NancyContext"/> instance containing the request/response context.</returns>
        NancyContext HandleRequest(Request request);

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Deletate to call when any errors occur</param>
        void HandleRequest(Request request, Action<NancyContext> onComplete, Action<Exception> onError);
    }
}