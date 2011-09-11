namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;
    using Xunit;

    public class AfterPipelineFixture
    {
        private AfterPipeline pipeline;

        private static NancyContext CreateContext()
        {
            return new NancyContext();
        }

        public AfterPipelineFixture()
        {
            pipeline = new AfterPipeline();
        }

        [Fact]
        public void PlusEquals_with_func_add_item_to_end_of_pipeline()
        {
            Action<NancyContext> item1 = (r) => { };
            Action<NancyContext> item2 = (r) => { };
            pipeline.AddItemToEndOfPipeline(item2);

            pipeline += item1;

            Assert.Equal(2, pipeline.PipelineDelegates.Count());
            Assert.Same(item1, pipeline.PipelineDelegates.Last());
        }

        [Fact]
        public void PlusEquals_with_another_pipeline_adds_those_pipeline_items_to_end_of_pipeline()
        {
            Action<NancyContext> item1 = (r) => { };
            Action<NancyContext> item2 = (r) => { };
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);
            Action<NancyContext> item3 = (r) => { };
            Action<NancyContext> item4 = (r) => { };
            var pipeline2 = new AfterPipeline();
            pipeline2.AddItemToEndOfPipeline(item3);
            pipeline2.AddItemToEndOfPipeline(item4);

            pipeline += pipeline2;

            Assert.Equal(4, pipeline.PipelineItems.Count());
            Assert.Same(item3, pipeline.PipelineDelegates.ElementAt(2));
            Assert.Same(item4, pipeline.PipelineDelegates.Last());
        }

        [Fact]
        public void When_cast_to_func_and_invoked_members_are_invoked()
        {
            var item1Called = false;
            Action<NancyContext> item1 = (r) => { item1Called = true; };
            var item2Called = false;
            Action<NancyContext> item2 = (r) => { item2Called = true; };
            var item3Called = false;
            Action<NancyContext> item3 = (r) => { item3Called = true; };
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(item3);

            Action<NancyContext> func = pipeline;
            func.Invoke(CreateContext());

            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.True(item3Called);
        }

        [Fact]
        public void When_cast_from_func_creates_a_pipeline_with_one_item()
        {
            Action<NancyContext> item1 = (r) => { };

            AfterPipeline castPipeline = item1;

            Assert.Equal(1, castPipeline.PipelineDelegates.Count());
            Assert.Same(item1, castPipeline.PipelineDelegates.First());
        }

        [Fact]
        public void Pipeline_containing_another_pipeline_will_invoke_items_in_both_pipelines()
        {
            var item1Called = false;
            Action<NancyContext> item1 = (r) => { item1Called = true; };
            var item2Called = false;
            Action<NancyContext> item2 = (r) => { item2Called = true; };
            var item3Called = false;
            Action<NancyContext> item3 = (r) => { item3Called = true; };
            var item4Called = false;
            Action<NancyContext> item4 = (r) => { item4Called = true; };
            pipeline += item1;
            pipeline += item2;
            var subPipeline = new AfterPipeline();
            subPipeline += item3;
            subPipeline += item4;

            pipeline.AddItemToEndOfPipeline(subPipeline);
            pipeline.Invoke(CreateContext());

            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.True(item3Called);
            Assert.True(item4Called);
        }
    }
}