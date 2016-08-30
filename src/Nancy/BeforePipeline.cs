namespace Nancy
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Intercepts the request before it is passed to the appropriate route handler.
    /// This gives you a couple of possibilities such as modifying parts of the request 
    /// or even prematurely aborting the request by returning a response that will be sent back to the caller.    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Nancy.AsyncNamedPipelineBase{System.Func{Nancy.NancyContext, System.Threading.CancellationToken,
    ///         System.Threading.Tasks.Task{Nancy.Response}}, System.Func{Nancy.NancyContext, Nancy.Response}}
    ///     </cref>
    /// </seealso>
    public class BeforePipeline : AsyncNamedPipelineBase<Func<NancyContext, CancellationToken, Task<Response>>, Func<NancyContext, Response>>
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforePipeline"/> class.
        /// </summary>
        public BeforePipeline()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforePipeline"/> class.
        /// </summary>
        /// <param name="capacity">Number of delegates in pipeline</param>
        public BeforePipeline(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="BeforePipeline"/> to <see>
        ///         <cref>Func{NancyContext, CancellationToken, Task{Response}}</cref>
        ///     </see>
        ///     .
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Func<NancyContext, CancellationToken, Task<Response>>(BeforePipeline pipeline)
        {
            return pipeline.Invoke;
        }

        /// <summary>
        /// Performs an implicit conversion from <see>
        ///         <cref>Func{NancyContext, CancellationToken, Task{Response}}</cref>
        ///     </see>
        ///     to <see cref="BeforePipeline"/>.
        /// </summary>
        /// <param name="func">The function.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator BeforePipeline(Func<NancyContext, CancellationToken, Task<Response>> func)
        {
            var pipeline = new BeforePipeline();
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }

        /// <summary>
        /// Appends a new function to the BeforePipeline.
        /// </summary>
        /// <param name="pipeline">The BeforePipeline pipeline instance.</param>
        /// <param name="func">The function.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static BeforePipeline operator +(BeforePipeline pipeline, Func<NancyContext, CancellationToken, Task<Response>> func)
        {
            pipeline.AddItemToEndOfPipeline(func);
            return pipeline;
        }


        /// <summary>
        /// Appends a new action to the BeforePipeline.
        /// </summary>
        /// <param name="pipeline">The BeforePipeline pipeline instance.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static BeforePipeline operator +(BeforePipeline pipeline, Func<NancyContext, Response> action)
        {
            pipeline.AddItemToEndOfPipeline(action);
            return pipeline;
        }


        /// <summary>
        /// Appends the items of an BeforePipeline to the other.
        /// </summary>
        /// <param name="pipelineToAddTo">The pipeline to add to.</param>
        /// <param name="pipelineToAdd">The pipeline to add.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static BeforePipeline operator +(BeforePipeline pipelineToAddTo, BeforePipeline pipelineToAdd)
        {
            foreach (var pipelineItem in pipelineToAdd.PipelineItems)
            {
                pipelineToAddTo.AddItemToEndOfPipeline(pipelineItem);
            }

            return pipelineToAddTo;
        }


        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
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
        /// Wraps the specified pipeline item into its async form.
        /// </summary>
        /// <param name="pipelineItem">The pipeline item.</param>
        /// <returns></returns>
        protected override PipelineItem<Func<NancyContext, CancellationToken, Task<Response>>> Wrap(PipelineItem<Func<NancyContext, Response>> pipelineItem)
        {
            return new PipelineItem<Func<NancyContext, CancellationToken, Task<Response>>>(pipelineItem.Name, (ctx, ct) => Task.FromResult(pipelineItem.Delegate(ctx)));
        }
    }
}
