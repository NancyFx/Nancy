namespace Nancy.Bootstrapper
{
    /// <summary>
    /// Defines the functionality for a factory that creates request pipelines.
    /// </summary>
    public interface IRequestPipelinesFactory
    {
        /// <summary>
        /// Creates and initializes a request pipeline.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> used by the request.</param>
        /// <returns>An <see cref="IPipelines"/> instance.</returns>
        IPipelines CreateRequestPipeline(NancyContext context);
    }
}