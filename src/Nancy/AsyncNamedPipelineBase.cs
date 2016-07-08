namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Abstract base class for request pipelines with async support
    /// </summary>
    /// <typeparam name="TAsyncDelegate"></typeparam>
    /// <typeparam name="TSyncDelegate"></typeparam>
    public abstract class AsyncNamedPipelineBase<TAsyncDelegate, TSyncDelegate>
    {
        /// <summary>
        /// Pipeline items to execute
        /// </summary>
        protected readonly List<PipelineItem<TAsyncDelegate>> pipelineItems;

        /// <summary>
        /// Creates a new instance of AsyncNamedPipelineBase
        /// </summary>
        protected AsyncNamedPipelineBase()
        {
            this.pipelineItems = new List<PipelineItem<TAsyncDelegate>>();
        }

        /// <summary>
        /// Creates a new instance of AsyncNamedPipelineBase with size
        /// </summary>
        /// <param name="capacity">Number of delegates in pipeline</param>
        protected AsyncNamedPipelineBase(int capacity)
        {
            this.pipelineItems = new List<PipelineItem<TAsyncDelegate>>(capacity);
        }

        /// <summary>
        /// Gets the current pipeline items
        /// </summary>
        public IEnumerable<PipelineItem<TAsyncDelegate>> PipelineItems
        {
            get { return this.pipelineItems.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the current pipeline item delegates
        /// </summary>
        public IEnumerable<TAsyncDelegate> PipelineDelegates
        {
            get { return this.pipelineItems.Select(pipelineItem => pipelineItem.Delegate); }
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToStartOfPipeline(TAsyncDelegate item)
        {
            this.AddItemToStartOfPipeline((PipelineItem<TAsyncDelegate>)item);
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToStartOfPipeline(TSyncDelegate item)
        {
            this.AddItemToStartOfPipeline(this.Wrap(item));
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="replaceInPlace">
        /// Whether to replace an existing item with the same name in its current place,
        /// rather than at the position requested. Defaults to false.
        /// </param>
        public virtual void AddItemToStartOfPipeline(PipelineItem<TAsyncDelegate> item, bool replaceInPlace = false)
        {
            this.InsertItemAtPipelineIndex(0, item, replaceInPlace);
        }

        /// <summary>
        /// Add an item to the start of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="replaceInPlace">
        /// Whether to replace an existing item with the same name in its current place,
        /// rather than at the position requested. Defaults to false.
        /// </param>
        public virtual void AddItemToStartOfPipeline(PipelineItem<TSyncDelegate> item, bool replaceInPlace = false)
        {
            this.AddItemToStartOfPipeline(this.Wrap(item), replaceInPlace);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToEndOfPipeline(TAsyncDelegate item)
        {
            this.AddItemToEndOfPipeline((PipelineItem<TAsyncDelegate>)item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToEndOfPipeline(TSyncDelegate item)
        {
            this.AddItemToEndOfPipeline(this.Wrap(item));
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="replaceInPlace">
        /// Whether to replace an existing item with the same name in its current place,
        /// rather than at the position requested. Defaults to false.
        /// </param>
        public virtual void AddItemToEndOfPipeline(PipelineItem<TAsyncDelegate> item, bool replaceInPlace = false)
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
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="replaceInPlace">
        /// Whether to replace an existing item with the same name in its current place,
        /// rather than at the position requested. Defaults to false.
        /// </param>
        public virtual void AddItemToEndOfPipeline(PipelineItem<TSyncDelegate> item, bool replaceInPlace = false)
        {
            this.AddItemToEndOfPipeline(this.Wrap(item), replaceInPlace);
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertItemAtPipelineIndex(int index, TAsyncDelegate item)
        {
            this.InsertItemAtPipelineIndex(index, (PipelineItem<TAsyncDelegate>)item);
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertItemAtPipelineIndex(int index, TSyncDelegate item)
        {
            this.InsertItemAtPipelineIndex(index, this.Wrap(item));
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
        public virtual void InsertItemAtPipelineIndex(int index, PipelineItem<TAsyncDelegate> item, bool replaceInPlace = false)
        {
            var existingIndex = this.RemoveByName(item.Name);

            var newIndex = (replaceInPlace && existingIndex != -1) ? existingIndex : index;

            this.pipelineItems.Insert(newIndex, item);
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
        public virtual void InsertItemAtPipelineIndex(int index, PipelineItem<TSyncDelegate> item, bool replaceInPlace = false)
        {
            this.InsertItemAtPipelineIndex(index, this.Wrap(item), replaceInPlace);
        }

        /// <summary>
        /// Insert an item before a named item.
        /// If the named item does not exist the item is inserted at the start of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert before</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertBefore(string name, TAsyncDelegate item)
        {
            this.InsertBefore(name, (PipelineItem<TAsyncDelegate>)item);
        }

        /// <summary>
        /// Insert an item before a named item.
        /// If the named item does not exist the item is inserted at the start of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert before</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertBefore(string name, TSyncDelegate item)
        {
            this.InsertBefore(name, this.Wrap(item));
        }

        /// <summary>
        /// Insert an item before a named item.
        /// If the named item does not exist the item is inserted at the start of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert before</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertBefore(string name, PipelineItem<TAsyncDelegate> item)
        {
            var existingIndex =
                this.pipelineItems.FindIndex(i => String.Equals(name, i.Name, StringComparison.Ordinal));

            if (existingIndex == -1)
            {
                existingIndex = 0;
            }

            this.InsertItemAtPipelineIndex(existingIndex, item);
        }

        /// <summary>
        /// Insert an item before a named item.
        /// If the named item does not exist the item is inserted at the start of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert before</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertBefore(string name, PipelineItem<TSyncDelegate> item)
        {
            this.InsertBefore(name, this.Wrap(item));
        }

        /// <summary>
        /// Insert an item after a named item.
        /// If the named item does not exist the item is inserted at the end of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert after</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertAfter(string name, TAsyncDelegate item)
        {
            this.InsertAfter(name, (PipelineItem<TAsyncDelegate>)item);
        }

        /// <summary>
        /// Insert an item after a named item.
        /// If the named item does not exist the item is inserted at the end of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert after</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertAfter(string name, TSyncDelegate item)
        {
            this.InsertAfter(name, this.Wrap(item));
        }

        /// <summary>
        /// Insert an item after a named item.
        /// If the named item does not exist the item is inserted at the end of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert after</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertAfter(string name, PipelineItem<TAsyncDelegate> item)
        {
            var existingIndex =
                this.pipelineItems.FindIndex(i => String.Equals(name, i.Name, StringComparison.Ordinal));

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
        /// Insert an item after a named item.
        /// If the named item does not exist the item is inserted at the end of the pipeline.
        /// </summary>
        /// <param name="name">Name of the item to insert after</param>
        /// <param name="item">Item to insert</param>
        public virtual void InsertAfter(string name, PipelineItem<TSyncDelegate> item)
        {
            this.InsertAfter(name, this.Wrap(item));
        }

        /// <summary>
        /// Remove a named pipeline item
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Index of item that was removed or -1 if nothing removed</returns>
        public virtual int RemoveByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return -1;
            }

            var existingIndex =
                this.pipelineItems.FindIndex(i => String.Equals(name, i.Name, StringComparison.Ordinal));

            if (existingIndex != -1)
            {
                this.pipelineItems.RemoveAt(existingIndex);
            }

            return existingIndex;
        }

        /// <summary>
        /// Wraps a sync delegate into it's async form
        /// </summary>
        /// <param name="syncDelegate">Sync pipeline instance</param>
        /// <returns>Async pipeline instance</returns>
        protected abstract PipelineItem<TAsyncDelegate> Wrap(PipelineItem<TSyncDelegate> syncDelegate);
    }
}