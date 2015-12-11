namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

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
            pipeline += r => { };

            pipeline.PipelineDelegates.ShouldHaveCount(1);
            Assert.Equal(1, pipeline.PipelineDelegates.Count());
        }

        [Fact]
        public void PlusEquals_with_another_pipeline_adds_those_pipeline_items_to_end_of_pipeline()
        {
            pipeline.AddItemToEndOfPipeline(r => { });
            pipeline.AddItemToEndOfPipeline(r => { });
            var pipeline2 = new AfterPipeline();
            pipeline2.AddItemToEndOfPipeline(r => { });
            pipeline2.AddItemToEndOfPipeline(r => { });

            pipeline += pipeline2;

            Assert.Equal(4, pipeline.PipelineItems.Count());
            Assert.Same(pipeline2.PipelineDelegates.ElementAt(0), pipeline.PipelineDelegates.ElementAt(2));
            Assert.Same(pipeline2.PipelineDelegates.ElementAt(1), pipeline.PipelineDelegates.Last());
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

            Action<NancyContext> action = context => { };
            pipeline += action;

            pipeline.Invoke(CreateContext(), CancellationToken.None);

            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.True(item3Called);
        }

        [Fact]
        public void When_cast_from_func_creates_a_pipeline_with_one_item()
        {
            var castPipeline = new AfterPipeline();
            castPipeline += r => { };

            Assert.Equal(1, castPipeline.PipelineDelegates.Count());
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
            pipeline.Invoke(CreateContext(), new CancellationToken());

            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.True(item3Called);
            Assert.True(item4Called);
        }

        [Fact]
        public void Pipeline_containing_method_returning_null_throws_InvalidOperationException()
        {
            pipeline.AddItemToEndOfPipeline(ReturnNull);

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                pipeline.Invoke(CreateContext(), new CancellationToken());
            });

            Assert.Equal("The after-pipeline action ReturnNull returned null; a Task was expected.", exception.Message);
        }

        [Fact]
        public void Pipeline_containing_lambda_returning_null_throws_InvalidOperationException()
        {
            pipeline.AddItemToEndOfPipeline((context, ct) => null);

            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                pipeline.Invoke(CreateContext(), new CancellationToken());
            });

            Assert.Equal("An after-pipeline action must not return null; a Task was expected.", exception.Message);
        }
        
        private static Task ReturnNull(NancyContext context, CancellationToken ct)
        {
            return null;
        }
    }
}