namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class NamedPipelineBase<TDelegate>
    {
        /// <summary>
        /// Pipeline items to execute
        /// </summary>
        protected readonly List<PipelineItem<TDelegate>> pipelineItems;

        protected NamedPipelineBase()
        {
            this.pipelineItems = new List<PipelineItem<TDelegate>>();
        }

        /// <summary>
        /// Gets the current pipeline items
        /// </summary>
        public IEnumerable<PipelineItem<TDelegate>> PipelineItems
        {
            get { return this.pipelineItems.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the current pipeline item delegates
        /// </summary>
        public IEnumerable<TDelegate> PipelineDelegates
        {
            get { return this.pipelineItems.Select(pipelineItem => pipelineItem.Delegate); }
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToStartOfPipeline(TDelegate item)
        {
            this.AddItemToStartOfPipeline((PipelineItem<TDelegate>)item);
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToStartOfPipeline(PipelineItem<TDelegate> item)
        {
            this.RemoveByName(item.Name);

            this.InsertItemAtPipelineIndex(0, item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToEndOfPipeline(TDelegate item)
        {
            this.AddItemToEndOfPipeline((PipelineItem<TDelegate>)item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToEndOfPipeline(PipelineItem<TDelegate> item)
        {
            this.RemoveByName(item.Name);

            this.pipelineItems.Add(item);
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertItemAtPipelineIndex(int index, TDelegate item)
        {
            this.InsertItemAtPipelineIndex(index, (PipelineItem<TDelegate>)item);
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertItemAtPipelineIndex(int index, PipelineItem<TDelegate> item)
        {
            this.RemoveByName(item.Name);

            this.pipelineItems.Insert(index, item);
        }

        public virtual void InsertBefore(string name, PipelineItem<TDelegate> item)
        {
            throw new NotImplementedException();
        }

        public virtual void InsertAfter(string name, PipelineItem<TDelegate> item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove a named pipeline item
        /// </summary>
        /// <param name="name">Name</param>
        public virtual void RemoveByName(string name)
        {
            this.pipelineItems.RemoveAll(i => String.Equals(name, i.Name, StringComparison.InvariantCulture));
        }
    }
}