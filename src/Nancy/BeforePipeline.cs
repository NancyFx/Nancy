namespace Nancy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Intercepts the request before it is passed to the appropriate route handler.
    /// This gives you a couple of possibilities such as modifying parts of the request 
    /// or even prematurely aborting the request by returning a response that will be sent back to the caller.
    /// </summary>
    public class BeforePipeline : AsyncNamedPipelineBase<Func<NancyContext, CancellationToken, Task<Response>>, Func<NancyContext, Response>>
    {
        /// <summary>
        /// Creates a new instance of BeforePipeline
        /// </summary>
        public BeforePipeline()
        {
        }

        /// <summary>
        /// Creates a new instance of BeforePipeline with a capacity
        /// </summary>
        /// <param name="capacity">Size of the pipeline which is the count of pipeline delegates</param>
        public BeforePipeline(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Implict type conversion operator from BeforePipeline to func
        /// </summary>
        /// <param name="pipeline"></param>
        public static implicit operator Func<NancyContext, CancellationToken, Task<Response>>(BeforePipeline pipeline)
        {
            return pipeline.Invoke;
        }

        /// <summary>
        /// Implict type conversion operator from func to BeforePipeline
        /// </summary>
        /// <param name="func"></param>
        public static implicit operator BeforePipeline(Func<NancyContext, CancellationToken, Task<Response>> func)
        {
            var pipeline = new BeforePipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        /// <summary>
        /// Appends a new func to the BeforePipeline
        /// </summary>
        /// <param name="pipeline">Target pipeline</param>
        /// <param name="func">A function that returns a task</param>
        /// <returns></returns>
        public static BeforePipeline operator +(BeforePipeline pipeline, Func<NancyContext, CancellationToken, Task<Response>> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }


        /// <summary>
        /// Appends a new action to the BeforePipeline
        /// </summary>
        /// <param name="pipeline">Target pipeline</param>
        /// <param name="action">Action to be carried out</param>
        /// <returns></returns>
        public static BeforePipeline operator +(BeforePipeline pipeline, Func<NancyContext, Response> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }

        /// <summary>
        /// Appends the items of an BeforePipeline to the other
        /// </summary>
        /// <param name="pipelineToAddTo"></param>
        /// <param name="pipelineToAdd"></param>
        /// <returns></returns>
        public static BeforePipeline operator +(BeforePipeline pipelineToAddTo, BeforePipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }


        /// <summary>
        /// Invokes BeforePipeline delegates
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
            return new PipelineItem<Func<NancyContext, CancellationToken, Task<Response>>>(pipelineItem.Name, (ctx, ct) => Task.FromResult(pipelineItem.Delegate(ctx)));
        }
    }
}
