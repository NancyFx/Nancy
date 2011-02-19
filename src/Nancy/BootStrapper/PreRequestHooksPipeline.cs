namespace Nancy.Bootstrapper
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
    public class PreRequestHooksPipeline
    {
        /// <summary>
        /// Pipeline items to execute
        /// </summary>
        private List<Func<NancyContext, Response>> pipelineItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreRequestHooksPipeline"/> class.
        /// </summary>
        public PreRequestHooksPipeline()
        {
            this.pipelineItems = new List<Func<NancyContext, Response>>();
        }

        /// <summary>
        /// Gets the current pipeline items
        /// </summary>
        public IEnumerable<Func<NancyContext, Response>> PipelineItems
        {
            get
            {
                return this.pipelineItems.AsReadOnly();
            }
        }

        public static implicit operator Func<NancyContext, Response>(PreRequestHooksPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator PreRequestHooksPipeline(Func<NancyContext, Response> func)
        {
            var pipeline = new PreRequestHooksPipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static PreRequestHooksPipeline operator +(PreRequestHooksPipeline pipeline, Func<NancyContext, Response> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static PreRequestHooksPipeline operator +(PreRequestHooksPipeline pipelineToAddTo, PreRequestHooksPipeline pipelineToAdd)
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
        /// <returns>
        /// Response from an item invocation, or null if no response was generated.
        /// </returns>
        public Response Invoke(NancyContext context)
        {
            Response returnValue = null;

            using (var enumerator = this.PipelineItems.GetEnumerator())
            {
                while (returnValue == null && enumerator.MoveNext())
                {
                    returnValue = enumerator.Current.Invoke(context);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItemToStartOfPipeline(Func<NancyContext, Response> item)
        {
            this.InsertItemAtPipelineIndex(0, item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItemToEndOfPipeline(Func<NancyContext, Response> item)
        {
            this.pipelineItems.Add(item);
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public void InsertItemAtPipelineIndex(int index, Func<NancyContext, Response> item)
        {
            this.pipelineItems.Insert(index, item);
        }
    }
}