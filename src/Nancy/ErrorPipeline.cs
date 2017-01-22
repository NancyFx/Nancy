namespace Nancy
{
    using System;

    /// <summary>
    /// A simple pipeline for on-error hooks.
    /// Hooks will be executed until either a hook returns a response, or every
    /// hook has been executed.
    /// Can be implictly cast to/from the on-error hook delegate signature
    /// (Func NancyContext, Exception, Response) for assigning to NancyEngine or for building
    /// composite pipelines.
    /// </summary>
    /// <seealso>
    ///     <cref>Nancy.NamedPipelineBase{System.Func{Nancy.NancyContext, System.Exception, dynamic}}</cref>
    /// </seealso>
    public class ErrorPipeline : NamedPipelineBase<Func<NancyContext, Exception, dynamic>>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorPipeline"/> class.
        /// </summary>
        public ErrorPipeline()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorPipeline"/> class, with
        /// the provided <paramref name="capacity"/>.
        /// </summary>
        /// <param name="capacity">The number of pipeline delegates.</param>
        public ErrorPipeline(int capacity) : base(capacity)
        {
        }


        /// <summary>
        /// Performs an implicit conversion from <see cref="ErrorPipeline"/> to <see cref="Func{NancyContext, Exception, dynamic}"/>.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Func<NancyContext, Exception, dynamic>(ErrorPipeline pipeline)
        {
            return pipeline.Invoke;
        }


        /// <summary>
        /// Performs an implicit conversion from <see cref="Func{NancyContext, Exception, dynamic}"/> to <see cref="ErrorPipeline"/>.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ErrorPipeline(Func<NancyContext, Exception, dynamic> func)
        {
            var pipeline = new ErrorPipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        /// <summary>
        /// Appends a new func to the ErrorPipeline.
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="func">The function.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static ErrorPipeline operator +(ErrorPipeline pipeline, Func<NancyContext, Exception, dynamic> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }


        /// <summary>
        /// Appends the items of an ErrorPipeline to the other.
        /// </summary>
        /// <param name="pipelineToAddTo">The pipeline to add to.</param>
        /// <param name="pipelineToAdd">The pipeline to add.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
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
        /// item returns a Response, or all items have been invoked.
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
        public dynamic Invoke(NancyContext context, Exception ex)
        {
            dynamic returnValue = null;

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