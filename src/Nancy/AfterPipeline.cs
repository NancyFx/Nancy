namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
    public class AfterPipeline
    {
        /// <summary>
        /// Pipeline items to execute
        /// </summary>
        protected List<PipelineItem<Action<NancyContext>>> pipelineItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterPipeline"/> class.
        /// </summary>
        public AfterPipeline()
        {
            this.pipelineItems = new List<PipelineItem<Action<NancyContext>>>();
        }

        /// <summary>
        /// Gets the current pipeline items
        /// </summary>
        public IEnumerable<Action<NancyContext>> PipelineItems
        {
            get
            {
                return this.pipelineItems.Select(pipelineItem => pipelineItem.Delegate);
            }
        }

        public static implicit operator Action<NancyContext>(AfterPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator AfterPipeline(Action<NancyContext> action)
        {
            var pipeline = new AfterPipeline();
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipeline, Action<NancyContext> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipelineToAddTo, AfterPipeline pipelineToAdd)
        {
            pipelineToAddTo.pipelineItems.AddRange(pipelineToAdd.pipelineItems);
            return pipelineToAddTo;
        }

        public void Invoke(NancyContext context)
        {
            foreach (var pipelineItem in this.PipelineItems)
            {
                pipelineItem.Invoke(context);
            }
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToStartOfPipeline(Action<NancyContext> item)
        {
            this.InsertItemAtPipelineIndex(0, item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToEndOfPipeline(Action<NancyContext> item)
        {
            this.pipelineItems.Add(item);
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertItemAtPipelineIndex(int index, Action<NancyContext> item)
        {
            this.pipelineItems.Insert(index, item);
        }
    }
}