namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using System.Linq;
    using Nancy.Bootstrapper;
    using Xunit;

    public class PipelinesFixture
    {
        [Fact]
        public void Should_create_default_after_request_hook_when_created_with_default_ctor()
        {
            // Given, When
            var pipelines = new Pipelines();

            // Then
            pipelines.AfterRequest.ShouldNotBeNull();
        }

        [Fact]
        public void Should_create_default_before_request_hook_when_created_with_default_ctor()
        {
            // Given, When
            var pipelines = new Pipelines();

            // Then
            pipelines.BeforeRequest.ShouldNotBeNull();
        }

        [Fact]
        public void Should_create_default_error_hook_when_created_with_default_ctor()
        {
            // Given, When
            var pipelines = new Pipelines();

            // Then
            pipelines.OnError.ShouldNotBeNull();
        }

        [Fact]
        public void Should_clone_after_request_hooks_when_created_with_existing_pipeline()
        {
            // Given
            Action<NancyContext> hook = ctx => ctx.Items.Add("foo", 1);

            var existing = new Pipelines();
            existing.AfterRequest.AddItemToEndOfPipeline(hook);

            // When
            var pipelines = new Pipelines(existing);

            // Then
            pipelines.AfterRequest.PipelineItems.First().Delegate.ShouldBeSameAs(hook);
        }

        [Fact]
        public void Should_clone_before_request_hooks_when_created_with_existing_pipeline()
        {
            // Given
            Func<NancyContext, Response> hook = ctx => null;

            var existing = new Pipelines();
            existing.BeforeRequest.AddItemToEndOfPipeline(hook);

            // When
            var pipelines = new Pipelines(existing);

            // Then
            pipelines.BeforeRequest.PipelineItems.First().Delegate.ShouldBeSameAs(hook);
        }

        [Fact]
        public void Should_clone_error_hooks_when_created_with_existing_pipeline()
        {
            // Given
            Func<NancyContext, Exception, Response> hook = (ctx, ex) => null;

            var existing = new Pipelines();
            existing.OnError.AddItemToEndOfPipeline(hook);

            // When
            var pipelines = new Pipelines(existing);

            // Then
            pipelines.OnError.PipelineItems.First().Delegate.ShouldBeSameAs(hook);
        }
    }
}