namespace Nancy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Nancy.Helpers;

    public class AfterPipeline : AsyncNamedPipelineBase<Func<NancyContext, CancellationToken, Task>, Action<NancyContext>>
    {
        private static readonly Task completeTask = TaskHelpers.CompletedTask;

        public AfterPipeline()
        {
        }

        public AfterPipeline(int capacity)
            : base(capacity)
        {
        }

        public static implicit operator Func<NancyContext, CancellationToken, Task>(AfterPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator AfterPipeline(Func<NancyContext, CancellationToken, Task> func)
        {
            var pipeline = new AfterPipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipeline, Func<NancyContext, CancellationToken, Task> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipeline, Action<NancyContext> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static AfterPipeline operator +(AfterPipeline pipelineToAddTo, AfterPipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }

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
            var syncDelegate = pipelineItem.Delegate;
            Func<NancyContext, CancellationToken, Task> asyncDelegate = (ctx, ct) =>
            {
                try
                {
                    syncDelegate.Invoke(ctx);
                    return completeTask;
                }
                catch (Exception e)
                {
                    var tcs = new TaskCompletionSource<object>();
                    tcs.SetException(e);
                    return tcs.Task;
                }
            };
            return new PipelineItem<Func<NancyContext, CancellationToken, Task>>(pipelineItem.Name, asyncDelegate);
        }
    }
}