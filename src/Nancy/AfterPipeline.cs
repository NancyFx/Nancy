namespace Nancy
{
    using System;
    using System.Linq;

    /// <summary>
    /// <para>
    /// A simple pipleline for post-request hooks.
    /// </para>
    /// <para>
    /// Can be implictly cast to/from the pre-request hook delegate signature
    /// (Func NancyContext, Response) for assigning to NancyEngine or for building
    /// composite pipelines.
    /// </para>
    /// </summary>
    public class AfterPipeline : NamedPipelineBase<Action<NancyContext>>
    {
        public AfterPipeline()
        {
        }

        public AfterPipeline(int capacity) : base(capacity)
        {
        }

        public static implicit operator Action<NancyContext>(AfterPipeline pipeline)
        {
            return pipeline.Invoke;
        }

        public static implicit operator AfterPipeline(Action<NancyContext> action)
        {
            var pipeline = new AfterPipeline();
            pipeline.AddItemToEndOfPipeline(action);
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

        public void Invoke(NancyContext context)
        {
            foreach (var pipelineItem in this.PipelineDelegates)
            {
                pipelineItem.Invoke(context);
            }
        }
    }
}