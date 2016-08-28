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
    public class AfterPipeline : AsyncNamedPipelineBase<Func<NancyContext, CancellationToken, Task>, Action<NancyContext>>
    {
        private static readonly Task completeTask = TaskHelpers.CompletedTask;

        /// <summary>
        /// Creates a new instance of AfterPipeline
        /// </summary>
        public AfterPipeline()
        {
        }
         
        /// <summary>
        /// Creates a new instance of AfterPipeline with a capacity
        /// </summary>
        /// <param name="capacity">Size of the pipeline which is the count of pipeline delegates</param>
        public AfterPipeline(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Implict type conversion operator from AfterPipeline to func
        /// </summary>
        /// <param name="pipeline"></param>
        public static implicit operator Func<NancyContext, CancellationToken, Task>(AfterPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        /// <summary>
        /// Implict type conversion operator from func to AfterPipeline
        /// </summary>
        /// <param name="func"></param>
        public static implicit operator AfterPipeline(Func<NancyContext, CancellationToken, Task> func)
        {
            var pipeline = new AfterPipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        /// <summary>
        /// Appends a new func to the AfterPipeline
        /// </summary>
        /// <param name="pipeline">Target pipeline</param>
        /// <param name="func">A function that returns a task</param>
        /// <returns></returns>
        public static AfterPipeline operator +(AfterPipeline pipeline, Func<NancyContext, CancellationToken, Task> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        /// <summary>
        /// Appends a new action to the AfterPipeline
        /// </summary>
        /// <param name="pipeline">Target pipeline</param>
        /// <param name="action">Action to be carried out</param>
        /// <returns></returns>
        public static AfterPipeline operator +(AfterPipeline pipeline, Action<NancyContext> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        /// <summary>
        /// Appends the items of an AfterPipeline to the other
        /// </summary>
        /// <param name="pipelineToAddTo"></param>
        /// <param name="pipelineToAdd"></param>
        /// <returns></returns>
        public static AfterPipeline operator +(AfterPipeline pipelineToAddTo, AfterPipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }

        /// <summary>
        /// Invokes AfterPipeline delegates
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Invoke(NancyContext context, CancellationToken cancellationToken)
        {
            foreach (var pipelineDelegate in this.PipelineDelegates)
            {
                await pipelineDelegate.Invoke(context, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Wraps a sync delegate into it's async form
        /// </summary>
        /// <param name="pipelineItem">Sync pipeline item instance</param>
        /// <returns>Async pipeline item instance</returns>
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
