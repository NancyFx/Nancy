namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;

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

        protected NamedPipelineBase(int capacity)
        {
            this.pipelineItems = new List<PipelineItem<TDelegate>>(capacity);
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
        /// <param name="replaceInPlace">
        /// Whether to replace an existing item with the same name in its current place,
        /// rather than at the position requested. Defaults to false.
        /// </param>
        public virtual void AddItemToStartOfPipeline(PipelineItem<TDelegate> item, bool replaceInPlace = false)
        {
            this.InsertItemAtPipelineIndex(0, item, replaceInPlace);
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
        /// <param name="replaceInPlace">
        /// Whether to replace an existing item with the same name in its current place,
        /// rather than at the position requested. Defaults to false.
        /// </param>
        public virtual void AddItemToEndOfPipeline(PipelineItem<TDelegate> item, bool replaceInPlace = false)
        {
            var existingIndex = this.RemoveByName(item.Name);

            if (replaceInPlace && existingIndex != -1)
            {
                this.InsertItemAtPipelineIndex(existingIndex, item);
            }
            else
            {
                this.pipelineItems.Add(item);
            }
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
        /// <param name="replaceInPlace">
        /// Whether to replace an existing item with the same name in its current place,
        /// rather than at the position requested. Defaults to false.
        /// </param>
        public virtual void InsertItemAtPipelineIndex(int index, PipelineItem<TDelegate> item, bool replaceInPlace = false)
        {
            var existingIndex = this.RemoveByName(item.Name);

            var newIndex = (replaceInPlace && existingIndex != -1) ? existingIndex : index;

            this.pipelineItems.Insert(newIndex, item);
        }

        /// <summary>
        /// Insert an item before a named item.
        /// If the named item does not exist the item is inserted at the start of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert before</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertBefore(string name, TDelegate item)
        {
            this.InsertBefore(name, (PipelineItem<TDelegate>)item);
        }

        /// <summary>
        /// Insert an item before a named item.
        /// If the named item does not exist the item is inserted at the start of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert before</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertBefore(string name, PipelineItem<TDelegate> item)
        {
            var existingIndex =
                this.pipelineItems.FindIndex(i => String.Equals(name, i.Name, StringComparison.InvariantCulture));

            if (existingIndex == -1)
            {
                existingIndex = 0;
            }

            this.InsertItemAtPipelineIndex(existingIndex, item);
        }

        /// <summary>
        /// Insert an item after a named item.
        /// If the named item does not exist the item is inserted at the end of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert after</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertAfter(string name, TDelegate item)
        {
            this.InsertAfter(name, (PipelineItem<TDelegate>)item);
        }

        /// <summary>
        /// Insert an item after a named item.
        /// If the named item does not exist the item is inserted at the end of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert after</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertAfter(string name, PipelineItem<TDelegate> item)
        {
            var existingIndex =
                this.pipelineItems.FindIndex(i => String.Equals(name, i.Name, StringComparison.InvariantCulture));

            if (existingIndex == -1)
            {
                existingIndex = this.pipelineItems.Count;
            }

            existingIndex++;

            if (existingIndex > this.pipelineItems.Count)
            {
                this.AddItemToEndOfPipeline(item);                
            }
            else
            {
                this.InsertItemAtPipelineIndex(existingIndex, item);
            }
        }

        /// <summary>
        /// Remove a named pipeline item
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Index of item that was removed or -1 if nothing removed</returns>
        public virtual int RemoveByName(string name)
        {
            var existingIndex =
                this.pipelineItems.FindIndex(i => String.Equals(name, i.Name, StringComparison.InvariantCulture));

            if (existingIndex != -1)
            {
                this.pipelineItems.RemoveAt(existingIndex);
            }

            return existingIndex;
        }
    }
}