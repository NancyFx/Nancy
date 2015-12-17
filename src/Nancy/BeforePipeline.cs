namespace Nancy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class BeforePipeline : AsyncNamedPipelineBase<Func<NancyContext, CancellationToken, Task<Response>>, Func<NancyContext, Response>>
    {
        public BeforePipeline()
        {
        }

        public BeforePipeline(int capacity)
            : base(capacity)
        {
        }

        public static implicit operator Func<NancyContext, CancellationToken, Task<Response>>(BeforePipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator BeforePipeline(Func<NancyContext, CancellationToken, Task<Response>> func)
        {
            var pipeline = new BeforePipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static BeforePipeline operator +(BeforePipeline pipeline, Func<NancyContext, CancellationToken, Task<Response>> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        public static BeforePipeline operator +(BeforePipeline pipeline, Func<NancyContext, Response> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        public static BeforePipeline operator +(BeforePipeline pipelineToAddTo, BeforePipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }

        public async Task<Response> Invoke(NancyContext context, CancellationToken cancellationToken)
        {
            foreach (var pipelineDelegate in this.PipelineDelegates)
            {
                var response = await pipelineDelegate.Invoke(context, cancellationToken).ConfigureAwait(false);
                if (response != null)
                {
                    return response;
                }
            }

            return null;
        }

        /// <summary>
        /// Wraps a sync delegate into it's async form
        /// </summary>
        /// <param name="pipelineItem">Sync pipeline item instance</param>
        /// <returns>Async pipeline item instance</returns>
        protected override PipelineItem<Func<NancyContext, CancellationToken, Task<Response>>> Wrap(PipelineItem<Func<NancyContext, Response>> pipelineItem)
        {
            var syncDelegate = pipelineItem.Delegate;
            Func<NancyContext, CancellationToken, Task<Response>> asyncDelegate = (ctx, ct) =>
            {
                var tcs = new TaskCompletionSource<Response>();
                try
                {
                    var result = syncDelegate.Invoke(ctx);
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                return tcs.Task;
            };
            return new PipelineItem<Func<NancyContext, CancellationToken, Task<Response>>>(pipelineItem.Name, asyncDelegate);
        }
    }
}
