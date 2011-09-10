namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
    public class BeforePipeline
    {
        /// <summary>
        /// Pipeline items to execute
        /// </summary>
        protected List<PipelineItem<Func<NancyContext, Response>>> pipelineItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforePipeline"/> class.
        /// </summary>
        public BeforePipeline()
        {
            this.pipelineItems = new List<PipelineItem<Func<NancyContext, Response>>>();
        }

        /// <summary>
        /// Gets the current pipeline items
        /// </summary>
        public IEnumerable<PipelineItem<Func<NancyContext, Response>>> PipelineItems
        {
            get { return this.pipelineItems.AsReadOnly(); }
        }

        /// <summary>
        /// Gets the current pipeline item delegates
        /// </summary>
        public IEnumerable<Func<NancyContext, Response>> PipelineDelegates
        {
            get { return this.pipelineItems.Select(pipelineItem => pipelineItem.Delegate); }
        }

        public static implicit operator Func<NancyContext, Response>(BeforePipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator BeforePipeline(Func<NancyContext, Response> func)
        {
            var pipeline = new BeforePipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static BeforePipeline operator +(BeforePipeline pipeline, Func<NancyContext, Response> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static BeforePipeline operator +(BeforePipeline pipelineToAddTo, BeforePipeline pipelineToAdd)
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
        public virtual Response Invoke(NancyContext context)
        {
            Response returnValue = null;

            using (var enumerator = this.PipelineDelegates.GetEnumerator())
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
        public virtual void AddItemToStartOfPipeline(Func<NancyContext, Response> item)
        {
            this.InsertItemAtPipelineIndex(0, item);
        }

        /// <summary>
        /// Add a named item to the start of the pipeline
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="item">Item to add</param>
        public virtual void AddNamedItemToStartOfPipeline(string name, Func<NancyContext, Response> item)
        {
            this.RemoveByName(name);

            this.InsertNamedItemAtPipelineIndex(0, name, item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void AddItemToEndOfPipeline(Func<NancyContext, Response> item)
        {
            this.pipelineItems.Add(item);
        }

        /// <summary>
        /// Add an item to the end of the pipeline
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="item">Item to add</param>
        public virtual void AddNamedItemToEndOfPipeline(string name, Func<NancyContext, Response> item)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null or empty", "name");
            }

            this.RemoveByName(name);

            this.pipelineItems.Add(new PipelineItem<Func<NancyContext, Response>>(name, item));
        }

        /// <summary>
        /// Add an item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertItemAtPipelineIndex(int index, Func<NancyContext, Response> item)
        {
            this.pipelineItems.Insert(index, item);
        }

        /// <summary>
        /// Add a named item to a specific place in the pipeline.
        /// </summary>
        /// <param name="index">Index to add at</param>
        /// <param name="name">Name</param>
        /// <param name="item">Item to add</param>
        public virtual void InsertNamedItemAtPipelineIndex(int index, string name, Func<NancyContext, Response> item)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("name cannot be null or empty", "name");
            }

            this.RemoveByName(name);

            this.pipelineItems.Insert(index, new PipelineItem<Func<NancyContext, Response>>(name, item));
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