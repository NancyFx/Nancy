namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;

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
            // Given, When
            pipeline += (ctx, ex) => null;
            pipeline += (ctx, ex) => null;

            // Then
            pipeline.PipelineDelegates.ShouldHaveCount(2);
        }

        [Fact]
        public void PlusEquals_with_another_pipeline_adds_those_pipeline_items_to_end_of_pipeline()
        {
            // Given
            pipeline.AddItemToEndOfPipeline((ctx, ex) => null);
            pipeline.AddItemToEndOfPipeline((ctx, ex) => null);
            var pipeline2 = new ErrorPipeline();
            pipeline2.AddItemToEndOfPipeline((ctx, ex) => null);
            pipeline2.AddItemToEndOfPipeline((ctx, ex) => null);

            // When
            pipeline += pipeline2;

            // Then
            pipeline.PipelineItems.ShouldHaveCount(4);
            pipeline2.PipelineDelegates.ElementAt(0).ShouldBeSameAs(pipeline.PipelineDelegates.ElementAt(2));
            pipeline2.PipelineDelegates.ElementAt(1).ShouldBeSameAs(pipeline.PipelineDelegates.Last());
        }

        [Fact]
        public void When_cast_to_func_and_invoked_members_are_invoked()
        {
            // Given
            var item1Called = false;
            Func<NancyContext, Exception, dynamic> item1 = (ctx, ex) => { item1Called = true; return null; };
            var item2Called = false;
            Func<NancyContext, Exception, dynamic> item2 = (ctx, ex) => { item2Called = true; return null; };
            var item3Called = false;
            Func<NancyContext, Exception, dynamic> item3 = (ctx, ex) => { item3Called = true; return null; };
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(item3);

            // When
            pipeline.Invoke(CreateContext(), new Exception());

            // Then
            item1Called.ShouldBeTrue();
            item2Called.ShouldBeTrue();
            item3Called.ShouldBeTrue();
        }

        [Fact]
        public void When_cast_from_func_creates_a_pipeline_with_one_item()
        {
            // Given
            var castPipeline = new ErrorPipeline();

            // When
            castPipeline += (ctx, ex) => null;

            // Then
            castPipeline.PipelineDelegates.ShouldHaveCount(1);
        }

        [Fact]
        public void Pipeline_containing_another_pipeline_will_invoke_items_in_both_pipelines()
        {
            // Given
            var item1Called = false;
            Func<NancyContext, Exception, dynamic> item1 = (ctx, ex) => { item1Called = true; return null; };
            var item2Called = false;
            Func<NancyContext, Exception, dynamic> item2 = (ctx, ex) => { item2Called = true; return null; };
            var item3Called = false;
            Func<NancyContext, Exception, dynamic> item3 = (ctx, ex) => { item3Called = true; return null; };
            var item4Called = false;
            Func<NancyContext, Exception, dynamic> item4 = (ctx, ex) => { item4Called = true; return null; };
            pipeline += item1;
            pipeline += item2;
            var subPipeline = new ErrorPipeline();
            subPipeline += item3;
            subPipeline += item4;

            // When
            pipeline.AddItemToEndOfPipeline(subPipeline);
            pipeline.Invoke(CreateContext(), new Exception());

            // Then
            item1Called.ShouldBeTrue();
            item2Called.ShouldBeTrue();
            item3Called.ShouldBeTrue();
            item4Called.ShouldBeTrue();
        }
    }
}