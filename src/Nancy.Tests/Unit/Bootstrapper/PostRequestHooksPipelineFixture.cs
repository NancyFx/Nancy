namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using System.Linq;
    using Nancy.Bootstrapper;
    using Xunit;

    public class PostRequestHooksPipelineFixture
    {
        private PostRequestHooksPipeline pipeline;

        private static NancyContext CreateContext()
        {
            return new NancyContext();
        }

        public PostRequestHooksPipelineFixture()
        {
            pipeline = new PostRequestHooksPipeline();
        }

        [Fact]
        public void AddItemToEndOfPipeline_adds_to_the_end_of_the_pipeline()
        {
            Action<NancyContext> item1 = (r) => { };
            Action<NancyContext> item2 = (r) => { };
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(item1);

            Assert.Equal(2, pipeline.PipelineItems.Count());
            Assert.Same(item1, pipeline.PipelineItems.Last());
        }

        [Fact]
        public void AddItemToStartOfPipeline_adds_to_the_end_of_the_pipeline()
        {
            Action<NancyContext> item1 = (r) => { };
            Action<NancyContext> item2 = (r) => { };
            pipeline.AddItemToEndOfPipeline(item2);

            pipeline.AddItemToStartOfPipeline(item1);

            Assert.Equal(2, pipeline.PipelineItems.Count());
            Assert.Same(item1, pipeline.PipelineItems.First());
        }

        [Fact]
        public void InsertItemAtPipelineIndex_adds_at_correct_index()
        {
            Action<NancyContext> item1 = (r) => { };
            Action<NancyContext> item2 = (r) => { };
            Action<NancyContext> item3 = (r) => { };
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item3);

            pipeline.InsertItemAtPipelineIndex(1, item2);

            Assert.Same(item1, pipeline.PipelineItems.ElementAt(0));
            Assert.Same(item2, pipeline.PipelineItems.ElementAt(1));
            Assert.Same(item3, pipeline.PipelineItems.ElementAt(2));
        }

        [Fact]
        public void PlusEquals_with_func_add_item_to_end_of_pipeline()
        {
            Action<NancyContext> item1 = (r) => { };
            Action<NancyContext> item2 = (r) => { };
            pipeline.AddItemToEndOfPipeline(item2);

            pipeline += item1;

            Assert.Equal(2, pipeline.PipelineItems.Count());
            Assert.Same(item1, pipeline.PipelineItems.Last());
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
            var pipeline2 = new PostRequestHooksPipeline();
            pipeline2.AddItemToEndOfPipeline(item3);
            pipeline2.AddItemToEndOfPipeline(item4);

            pipeline += pipeline2;

            Assert.Equal(4, pipeline.PipelineItems.Count());
            Assert.Same(item3, pipeline.PipelineItems.ElementAt(2));
            Assert.Same(item4, pipeline.PipelineItems.Last());
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

            PostRequestHooksPipeline castPipeline = item1;

            Assert.Equal(1, castPipeline.PipelineItems.Count());
            Assert.Same(item1, castPipeline.PipelineItems.First());
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
            var subPipeline = new PostRequestHooksPipeline();
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