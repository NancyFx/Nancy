namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Xunit;

    public class BeforePipelineFixture
    {
        private BeforePipeline pipeline;

        private static Response CreateResponse()
        {
            return new Response();
        }

        private static NancyContext CreateContext()
        {
            return new NancyContext();
        }

        public BeforePipelineFixture()
        {
            this.pipeline = new BeforePipeline();
        }

        [Fact]
        public void When_invoked_pipeline_member_returning_a_response_stops_pipeline_execution()
        {
            // Given
            var item1Called = false;
            Func<NancyContext, Response> item1 = (r) => { item1Called = true; return null; };
            var item2Called = false;
            Func<NancyContext, Response> item2 = (r) => { item2Called = true; return CreateResponse(); };
            var item3Called = false;
            Func<NancyContext, Response> item3 = (r) => { item3Called = true; return null; };
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(item3);

            // When
            pipeline.Invoke(CreateContext(), new CancellationToken());

            // Then
            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.False(item3Called);
        }

        [Fact]
        public void When_invoked_pipeline_member_returning_a_response_returns_that_response()
        {
            // Given
            var response = CreateResponse();
            Func<NancyContext, Response> item1 = (r) => null;
            Func<NancyContext, Response> item2 = (r) => response;
            Func<NancyContext, Response> item3 = (r) => null;
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(item3);
            
            // When
            var result = pipeline.Invoke(CreateContext(), new CancellationToken());

            // Then
            Assert.Same(response, result.Result);
        }

        [Fact]
        public void When_invoked_pipeline_members_all_return_null_returns_null()
        {
            // Given
            Func<NancyContext, Response> item1 = (r) => null;
            Func<NancyContext, Response> item2 = (r) => null;
            Func<NancyContext, Response> item3 = (r) => null;
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(item3);

            // When
            var result = pipeline.Invoke(CreateContext(), new CancellationToken());

            // Then
            Assert.Null(result.Result);
        }

        [Fact]
        public void PlusEquals_with_func_add_item_to_end_of_pipeline()
        {
            // Given
            pipeline.AddItemToEndOfPipeline(r => CreateResponse());

            // When
            pipeline += r => null;

            // Then
            Assert.Equal(2, pipeline.PipelineDelegates.Count());
        }

        [Fact]
        public void PlusEquals_with_another_pipeline_adds_those_pipeline_items_to_end_of_pipeline()
        {
            // Given
            pipeline.AddItemToEndOfPipeline(r => null);
            pipeline.AddItemToEndOfPipeline(r => CreateResponse());
            var pipeline2 = new BeforePipeline();
            pipeline2.AddItemToEndOfPipeline(r => null);
            pipeline2.AddItemToEndOfPipeline(r => CreateResponse());

            // When
            pipeline += pipeline2;

            // Then
            Assert.Equal(4, pipeline.PipelineItems.Count());
            Assert.Same(pipeline2.PipelineDelegates.ElementAt(0), pipeline.PipelineDelegates.ElementAt(2));
            Assert.Same(pipeline2.PipelineDelegates.ElementAt(1), pipeline.PipelineDelegates.Last());
        }

        [Fact]
        public void When_cast_to_func_and_invoked_members_are_invoked()
        {
            // Given
            var item1Called = false;
            Func<NancyContext, Response> item1 = r => { item1Called = true; return null; };
            var item2Called = false;               
            Func<NancyContext, Response> item2 = r => { item2Called = true; return null; };
            var item3Called = false;               
            Func<NancyContext, Response> item3 = r => { item3Called = true; return null; };
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(item3);

            // When
            Func<NancyContext, CancellationToken, Task<Response>> func = pipeline;
            func.Invoke(CreateContext(), new CancellationToken());

            // Then
            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.True(item3Called);
        }

        [Fact]
        public void When_cast_from_func_creates_a_pipeline_with_one_item()
        {
            // Given
            Func <NancyContext, CancellationToken,Task<Response>> item2 = (token, task) => null;

            // When
            BeforePipeline castPipeline = item2;

            // Then
            Assert.Equal(1, castPipeline.PipelineDelegates.Count());
            Assert.Same(item2, castPipeline.PipelineDelegates.First());
        }


        [Fact]
        public void Should_be_able_to_throw_exception_from_async_pipeline_item()
        {
            // Given
            Func<NancyContext, CancellationToken, Task<Response>> pipeLineItem = (ctx, token) => new Task<Response>(() =>
            {
                Thread.Sleep(1000);
                throw new Exception("aaarg");
            });

            // When
            pipeline.AddItemToStartOfPipeline(pipeLineItem);

            // Then
            Assert.Throws<AggregateException>(() => pipeline.Invoke(CreateContext(), new CancellationToken()).Result);
        }


        [Fact]
        public void Pipeline_containing_another_pipeline_will_invoke_items_in_both_pipelines()
        {
            // Given
            var item1Called = false;
            Func<NancyContext, Response> item1 = (r) => { item1Called = true; return null; };
            var item2Called = false;
            Func<NancyContext, Response> item2 = (r) => { item2Called = true; return null; };
            var item3Called = false;
            Func<NancyContext, Response> item3 = (r) => { item3Called = true; return null; };
            var item4Called = false;
            Func<NancyContext, Response> item4 = (r) => { item4Called = true; return null; };
            pipeline += item1;
            pipeline += item2;
            var subPipeline = new BeforePipeline();
            subPipeline += item3;
            subPipeline += item4;

            // When
            pipeline.AddItemToEndOfPipeline(subPipeline);
            pipeline.Invoke(CreateContext(), new CancellationToken());

            // Then
            Assert.True(item1Called);
            Assert.True(item2Called);
            Assert.True(item3Called);
            Assert.True(item4Called);
        }
    }
}