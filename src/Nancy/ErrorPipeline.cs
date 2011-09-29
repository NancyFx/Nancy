namespace Nancy
{
    using System;
    using System.Collections.Generic;

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
    public class ErrorPipeline
    {
        /// <summary>
        /// Pipeline items to execute
        /// </summary>
        private List<Func<NancyContext, Exception, Response>> pipelineItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorPipeline"/> class.
        /// </summary>
        public ErrorPipeline()
        {
            this.pipelineItems = new List<Func<NancyContext, Exception, Response>>();
        }

        /// <summary>
        /// Gets the current pipeline items
        /// </summary>
        public IEnumerable<Func<NancyContext, Exception, Response>> PipelineItems
        {
            get
            {
                return this.pipelineItems.AsReadOnly();
            }
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
            pipelineToAddTo.pipelineItems.AddRange(pipelineToAdd.pipelineItems);
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
        public Response Invoke(NancyContext context, Exception ex)
        {
            foreach (var pipelineItem in this.pipelineItems)
            {
                var response = pipelineItem.Invoke(context, ex);
                if (response != null)
                { 
                    return response;
                }
            }
            if (context.Response != null)
            { 
                return context.Response;
            }
            throw ex;
        }
        
        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItemToStartOfPipeline(Func<NancyContext, Exception, Response> item)
        {
            this.InsertItemAtPipelineIndex(0, item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItemToEndOfPipeline(Func<NancyContext, Exception, Response> item)
        {
            this.pipelineItems.Add(item);
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public void InsertItemAtPipelineIndex(int index, Func<NancyContext, Exception, Response> item)
        {
            this.pipelineItems.Insert(index, item);
        }
    }
}