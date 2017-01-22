namespace Nancy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Nancy.Helpers;

    /// <summary>
    /// Intercepts the request after the appropriate route handler has completed its operation.
    /// The After hooks does not have any return value because one has already been produced by the appropriate route.
    /// Instead you get the option to modify (or completely replace) the existing response by accessing the Response property of the NancyContext that is passed in.
    /// </summary>
    /// <seealso cref="AsyncNamedPipelineBase{TAsyncDelegate,TSyncDelegate}" />
    public class AfterPipeline : AsyncNamedPipelineBase<Func<NancyContext, CancellationToken, Task>, Action<NancyContext>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AfterPipeline"/> class.
        /// </summary>
        public AfterPipeline()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AfterPipeline"/> class, with
        /// the provided <paramref name= "capacity" />.
        /// </summary>
        /// <param name="capacity">Number of delegates in pipeline</param>
        public AfterPipeline(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="AfterPipeline"/> to <see cref="Func{NancyContext, CancellationToken, Task}"/>.
        /// </summary>
        /// <param name="pipeline">The <see cref="AfterPipeline"/> instance.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Func<NancyContext, CancellationToken, Task>(AfterPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Func{NancyContext, CancellationToken, Task}"/> to <see cref="AfterPipeline"/>.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>
        /// A new <see cref="AfterPipeline"/> instance with <paramref name="func"/>
        /// </returns>
        public static implicit operator AfterPipeline(Func<NancyContext, CancellationToken, Task> func)
        {
            var pipeline = new AfterPipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        /// <summary>
        /// Appends a new func to the AfterPipeline
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="func">The function.</param>
        /// <returns>
        /// <paramref name="pipeline"/>
        /// </returns>
        public static AfterPipeline operator +(AfterPipeline pipeline, Func<NancyContext, CancellationToken, Task> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        /// <summary>
        /// Appends a new action to the AfterPipeline
        /// </summary>
        /// <param name="pipeline">The <see cref="AfterPipeline"/> instance.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// <paramref name="pipeline"/>
        /// </returns>
        public static AfterPipeline operator +(AfterPipeline pipeline, Action<NancyContext> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        /// <summary>
        /// Appends the items of an <see cref="AfterPipeline"/> to the other.
        /// </summary>
        /// <param name="pipelineToAddTo">The <see cref="AfterPipeline"/> to add to.</param>
        /// <param name="pipelineToAdd">The <see cref="AfterPipeline"/> to add.</param>
        /// <returns>
        /// <paramref name="pipelineToAddTo"/>
        /// </returns>
        public static AfterPipeline operator +(AfterPipeline pipelineToAddTo, AfterPipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }

        /// <summary>
        /// Invokes the pipeline items in Nancy context.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> instance.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> instance</param>
        public async Task Invoke(NancyContext context, CancellationToken cancellationToken)
        {
            foreach (var pipelineDelegate in this.PipelineDelegates)
            {
                await pipelineDelegate.Invoke(context, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Wraps the specified <see cref="PipelineItem{T}"/> instance into its async form.
        /// </summary>
        /// <param name="pipelineItem">The <see cref="PipelineItem{T}"/> instance.</param>
        /// <returns>Async <see cref="PipelineItem{T}"/> instance</returns>
        protected override PipelineItem<Func<NancyContext, CancellationToken, Task>> Wrap(PipelineItem<Action<NancyContext>> pipelineItem)
        {
            return new PipelineItem<Func<NancyContext, CancellationToken, Task>>(pipelineItem.Name, (ctx, ct) =>
            {
                pipelineItem.Delegate(ctx);
                return TaskHelpers.CompletedTask;
            });
        }
    }
}
