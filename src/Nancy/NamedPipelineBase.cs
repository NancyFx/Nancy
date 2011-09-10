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
        private readonly List<PipelineItem<TDelegate>> pipelineItems;

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
            this.InsertItemAtPipelineIndex(0, item);
        }

        /// <summary>
        /// Add a named item to the start of the pipeline
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="item">Item to add</param>
        public virtual void AddNamedItemToStartOfPipeline(string name, TDelegate item)
        {
            this.RemoveByName(name);

            this.InsertNamedItemAtPipelineIndex(0, name, item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToEndOfPipeline(TDelegate item)
        {
            this.pipelineItems.Add(item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="item">Item to add</param>
        public virtual void AddNamedItemToEndOfPipeline(string name, TDelegate item)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null or empty", "name");
            }

            this.RemoveByName(name);

            this.pipelineItems.Add(new PipelineItem<TDelegate>(name, item));
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertItemAtPipelineIndex(int index, TDelegate item)
        {
            this.pipelineItems.Insert(index, item);
        }

        /// <summary>
        /// Add a named item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="name">Name</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertNamedItemAtPipelineIndex(int index, string name, TDelegate item)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null or empty", "name");
            }

            this.RemoveByName(name);

            this.pipelineItems.Insert(index, new PipelineItem<TDelegate>(name, item));
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