namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;
    using System.Threading;
    using Xunit;

    public class ErrorPipelineFixture
    {
        private ErrorPipeline pipeline;

        private static NancyContext CreateContext()
        {
            return new NancyContext();
        }

        public ErrorPipelineFixture()
        {
            pipeline = new ErrorPipeline();
        }

        [Fact]
        public void PlusEquals_with_func_adds_item_to_end_of_pipeline()
        {
        	pipeline += (ctx, ex) => null;
        	pipeline += (ctx, ex) => null;

            Assert.Equal(2, pipeline.PipelineDelegates.Count());
        }

        [Fact]
        public void PlusEquals_with_another_pipeline_adds_those_pipeline_items_to_end_of_pipeline()
        {
            pipeline.AddItemToEndOfPipeline((ctx, ex) => null);
            pipeline.AddItemToEndOfPipeline((ctx, ex) => null);
            var pipeline2 = new ErrorPipeline();
            pipeline2.AddItemToEndOfPipeline((ctx, ex) => null);
            pipeline2.AddItemToEndOfPipeline((ctx, ex) => null);

            pipeline += pipeline2;

            Assert.Equal(4, pipeline.PipelineItems.Count());
            Assert.Same(pipeline2.PipelineDelegates.ElementAt(0), pipeline.PipelineDelegates.ElementAt(2));
            Assert.Same(pipeline2.PipelineDelegates.ElementAt(1), pipeline.PipelineDelegates.Last());
        }

        [Fact]
        public void When_cast_to_func_and_invoked_members_are_invoked()
        {
            var item1Called = false;
            Func<NancyContext, Exception, Response> item1 = (ctx, ex) => { item1Called = true; return null; };
            var item2Called = false;
            Func<NancyContext, Exception, Response> item2 = (ctx, ex) => { item2Called = true; return null; };
            var item3Called = false;
            Func<NancyContext, Exception, Response> item3 = (ctx, ex) => { item3Called = true; return null; };
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(item3);

            Func<NancyContext, Exception, Response> func = (ctx, ex) => null;
            pipeline += func;

            pipeline.Invoke(CreateContext(), new Exception());

            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.True(item3Called);
        }

        [Fact]
        public void When_cast_from_func_creates_a_pipeline_with_one_item()
        {
            var castPipeline = new ErrorPipeline();
            castPipeline += (ctx, ex) => null;

            Assert.Equal(1, castPipeline.PipelineDelegates.Count());
        }

        [Fact]
        public void Pipeline_containing_another_pipeline_will_invoke_items_in_both_pipelines()
        {
            var item1Called = false;
            Func<NancyContext, Exception, Response> item1 = (ctx, ex) => { item1Called = true; return null; };
            var item2Called = false;
            Func<NancyContext, Exception, Response> item2 = (ctx, ex) => { item2Called = true; return null; };
            var item3Called = false;
            Func<NancyContext, Exception, Response> item3 = (ctx, ex) => { item3Called = true; return null; };
            var item4Called = false;
            Func<NancyContext, Exception, Response> item4 = (ctx, ex) => { item4Called = true; return null; };
            pipeline += item1;
            pipeline += item2;
            var subPipeline = new ErrorPipeline();
            subPipeline += item3;
            subPipeline += item4;

            pipeline.AddItemToEndOfPipeline(subPipeline);
            pipeline.Invoke(CreateContext(), new Exception());

            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.True(item3Called);
            Assert.True(item4Called);
        }
    }
}