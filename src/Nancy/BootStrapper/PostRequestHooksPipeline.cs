namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// <para>
    /// A simple pipleline for post-request hooks.
    /// </para>
    /// <para>
    /// Can be implictly cast to/from the pre-request hook delegate signature
    /// (Func NancyContext, Response) for assigning to NancyEngine or for building
    /// composite pipelines.
    /// </para>
    /// </summary>
    public class PostRequestHooksPipeline
    {
        /// <summary>
        /// Pipeline items to execute
        /// </summary>
        private List<Action<NancyContext>> pipelineItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="PostRequestHooksPipeline"/> class.
        /// </summary>
        public PostRequestHooksPipeline()
        {
            this.pipelineItems = new List<Action<NancyContext>>();
        }

        /// <summary>
        /// Gets the current pipeline items
        /// </summary>
        public IEnumerable<Action<NancyContext>> PipelineItems
        {
            get
            {
                return this.pipelineItems.AsReadOnly();
            }
        }

        public static implicit operator Action<NancyContext>(PostRequestHooksPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator PostRequestHooksPipeline(Action<NancyContext> action)
        {
            var pipeline = new PostRequestHooksPipeline();
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static PostRequestHooksPipeline operator +(PostRequestHooksPipeline pipeline, Action<NancyContext> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static PostRequestHooksPipeline operator +(PostRequestHooksPipeline pipelineToAddTo, PostRequestHooksPipeline pipelineToAdd)
        {
            pipelineToAddTo.pipelineItems.AddRange(pipelineToAdd.pipelineItems);
            return pipelineToAddTo;
        }

        public void Invoke(NancyContext context)
        {
            foreach (var pipelineItem in this.pipelineItems)
            {
                pipelineItem.Invoke(context);
            }
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItemToStartOfPipeline(Action<NancyContext> item)
        {
            this.InsertItemAtPipelineIndex(0, item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public void AddItemToEndOfPipeline(Action<NancyContext> item)
        {
            this.pipelineItems.Add(item);
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public void InsertItemAtPipelineIndex(int index, Action<NancyContext> item)
        {
            this.pipelineItems.Insert(index, item);
        }
    }
}