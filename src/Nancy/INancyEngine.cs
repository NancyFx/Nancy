namespace Nancy
{
    using System;
    using System.Threading.Tasks;

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
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <returns>A Task representing the </returns>
        Task<NancyContext> HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest);
    }
}