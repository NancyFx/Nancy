namespace Nancy
{
    using System;

    /// <summary>
    /// <para>
    /// A simple pipeline for pre-request hooks.
    /// Hooks will be executed until either a hook returns a response, or every
    /// hook has been executed.
    /// </para>
    /// <para>
    /// Can be implictly cast to/from the pre-request hook delegate signature
    /// (Func NancyContext, Response) for assigning to NancyEngine or for building
    /// composite pipelines.
    /// </para>
    /// </summary>
    public class ErrorPipeline : NamedPipelineBase<Func<NancyContext, Exception, Response>>
    {
        public ErrorPipeline()
        {
        }

        public ErrorPipeline(int capacity) : base(capacity)
        {
        }

        public static implicit operator Func<NancyContext, Exception, Response>(ErrorPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator ErrorPipeline(Func<NancyContext, Exception, Response> func)
        {
            var pipeline = new ErrorPipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static ErrorPipeline operator +(ErrorPipeline pipeline, Func<NancyContext, Exception, Response> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static ErrorPipeline operator +(ErrorPipeline pipelineToAddTo, ErrorPipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }

        /// <summary>
        /// Invoke the pipeline. Each item will be invoked in turn until either an
        /// item returns a Response, or all items have beene invoked.
        /// </summary>
        /// <param name="context">
        /// The current context to pass to the items.
        /// </param>
        /// <param name="ex">
        /// The exception currently being handled by the error pipeline
        /// </param>
        /// <returns>
        /// Response from an item invocation, or null if no response was generated.
        /// </returns>
        public Response Invoke(NancyContext context, Exception ex)
        {
            Response returnValue = null;

            using (var enumerator = this.PipelineDelegates.GetEnumerator())
            {
                while (returnValue == null && enumerator.MoveNext())
                {
                    returnValue = enumerator.Current.Invoke(context, ex);
                }
            }

            return returnValue;
        }
    }
}