namespace Nancy.Bootstrapper
{
    using System.Linq;

    /// <summary>
    /// Default implementation of the Nancy pipelines
    /// </summary>
    public class Pipelines : IPipelines
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Pipelines"/> class.
        /// </summary>
        public Pipelines()
        {
            this.AfterRequest = new AfterPipeline();
            this.BeforeRequest = new BeforePipeline();
            this.OnError = new ErrorPipeline();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pipelines"/> class and clones the hooks from
        /// the provided <see cref="IPipelines"/> instance.
        /// </summary>
        public Pipelines(IPipelines pipelines)
        {
            this.AfterRequest = 
                new AfterPipeline(pipelines.AfterRequest.PipelineItems.Count());

            foreach (var pipelineItem in pipelines.AfterRequest.PipelineItems)
            {
                this.AfterRequest.AddItemToEndOfPipeline(pipelineItem);
            }

            this.BeforeRequest = 
                new BeforePipeline(pipelines.BeforeRequest.PipelineItems.Count());

            foreach (var pipelineItem in pipelines.BeforeRequest.PipelineItems)
            {
                this.BeforeRequest.AddItemToEndOfPipeline(pipelineItem);
            }

            this.OnError = 
                new ErrorPipeline(pipelines.OnError.PipelineItems.Count());

            foreach (var pipelineItem in pipelines.OnError.PipelineItems)
            {
                this.OnError.AddItemToEndOfPipeline(pipelineItem);
            }
        }

        /// <summary>
        /// <para>
        /// The pre-request hook
        /// </para>
        /// <para>
        /// The PreRequest hook is called prior to processing a request. If a hook returns
        /// a non-null response then processing is aborted and the response provided is
        /// returned.
        /// </para>
        /// </summary>
        public BeforePipeline BeforeRequest { get; set; }

        /// <summary>
        /// <para>
        /// The post-request hook
        /// </para>
        /// <para>
        /// The post-request hook is called after the response is created. It can be used
        /// to rewrite the response or add/remove items from the context.
        /// </para>
        /// </summary>
        public AfterPipeline AfterRequest { get; set; }

        /// <summary>
        /// <para>
        /// The error hook
        /// </para>
        /// <para>
        /// The error hook is called if an exception is thrown at any time during the pipeline.
        /// If no error hook exists a standard InternalServerError response is returned
        /// </para>
        /// </summary>
        public ErrorPipeline OnError { get; set; }
    }
}