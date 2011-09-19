namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class NamedPipelineBaseFixture
    {
        private class TestPipeline :  NamedPipelineBase<Action<string>>
        {
            public IEnumerable<PipelineItem<Action<string>>> Items
            {
                get { return pipelineItems; }
            }
        }

        private readonly TestPipeline pipeline;

        public NamedPipelineBaseFixture()
        {
            this.pipeline = new TestPipeline();
        }

        [Fact]
        public void Should_be_able_to_add_to_start_of_pipeline()
        {
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            pipeline.AddItemToEndOfPipeline(item2);

            pipeline.AddItemToStartOfPipeline(item1);

            Assert.Equal(2, pipeline.Items.Count());
            Assert.Same(item1, pipeline.Items.First());
        }

        [Fact]
        public void Should_be_able_to_add_to_end_of_pipeline()
        {
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            pipeline.AddItemToEndOfPipeline(item2);

            pipeline.AddItemToEndOfPipeline(item1);

            Assert.Equal(2, pipeline.Items.Count());
            Assert.Same(item1, pipeline.Items.Last());
        }

        [Fact]
        public void Should_be_able_to_add_at_specific_index()
        {
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            var item3 = new PipelineItem<Action<string>>("item3", s => { });
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item3);

            pipeline.InsertItemAtPipelineIndex(1, item2);

            Assert.Same(item1, pipeline.Items.ElementAt(0));
            Assert.Same(item2, pipeline.Items.ElementAt(1));
            Assert.Same(item3, pipeline.Items.ElementAt(2));
        }

        [Fact]
        public void Should_remove_item_with_same_name_when_adding_to_start()
        {
            var existingItem = new PipelineItem<Action<string>>("item1", s => { });
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            pipeline.AddItemToStartOfPipeline(existingItem);
            pipeline.AddItemToEndOfPipeline(item2);

            pipeline.AddItemToStartOfPipeline(item1);

            Assert.Equal(2, pipeline.Items.Count());
            Assert.Same(item1, pipeline.Items.First());
        }

        [Fact]
        public void Should_remove_item_with_same_name_when_adding_to_end()
        {
            var existingItem = new PipelineItem<Action<string>>("item1", s => { });
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(existingItem);

            pipeline.AddItemToEndOfPipeline(item1);

            Assert.Equal(2, pipeline.Items.Count());
            Assert.Same(item1, pipeline.Items.Last());
        }

        [Fact]
        public void Should_remove_item_with_same_name_when_adding_at_index()
        {
            var existingItem = new PipelineItem<Action<string>>("item2", s => { });
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            var item3 = new PipelineItem<Action<string>>("item3", s => { });
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item3);
            pipeline.AddItemToEndOfPipeline(existingItem);

            pipeline.InsertItemAtPipelineIndex(1, item2);

            Assert.Same(item1, pipeline.Items.ElementAt(0));
            Assert.Same(item2, pipeline.Items.ElementAt(1));
            Assert.Same(item3, pipeline.Items.ElementAt(2));
        }

        [Fact]
        public void Should_be_able_to_insert_before_a_named_item()
        {
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            var item3 = new PipelineItem<Action<string>>("item3", s => { });
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item3);

            pipeline.InsertBefore("item3", item2);

            Assert.Same(item1, pipeline.Items.ElementAt(0));
            Assert.Same(item2, pipeline.Items.ElementAt(1));
            Assert.Same(item3, pipeline.Items.ElementAt(2));
        }

        [Fact]
        public void Should_be_able_to_insert_after_a_named_item()
        {
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            var item3 = new PipelineItem<Action<string>>("item3", s => { });
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item3);

            pipeline.InsertAfter("item1", item2);

            Assert.Same(item1, pipeline.Items.ElementAt(0));
            Assert.Same(item2, pipeline.Items.ElementAt(1));
            Assert.Same(item3, pipeline.Items.ElementAt(2));
        }

        [Fact]
        public void Should_add_to_start_if_inserting_before_and_item_doesnt_exist()
        {
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            var item3 = new PipelineItem<Action<string>>("item3", s => { });
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item3);

            pipeline.InsertBefore("nonexistant", item2);

            Assert.Same(item2, pipeline.Items.ElementAt(0));
            Assert.Same(item1, pipeline.Items.ElementAt(1));
            Assert.Same(item3, pipeline.Items.ElementAt(2));
        }

        [Fact]
        public void Should_add_to_end_if_inserting_after_and_item_doesnt_exist()
        {
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            var item3 = new PipelineItem<Action<string>>("item3", s => { });
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item3);

            pipeline.InsertAfter("nonexistant", item2);

            Assert.Same(item1, pipeline.Items.ElementAt(0));
            Assert.Same(item3, pipeline.Items.ElementAt(1));
            Assert.Same(item2, pipeline.Items.ElementAt(2));
        }

        [Fact]
        public void Should_replace_in_place_if_set_when_adding_to_start()
        {
            var existingItem = new PipelineItem<Action<string>>("item1", s => { });
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(existingItem);

            pipeline.AddItemToStartOfPipeline(item1, true);

            Assert.Equal(2, pipeline.Items.Count());
            Assert.Same(item1, pipeline.Items.Last());
        }

        [Fact]
        public void Should_replace_in_place_if_set_when_adding_to_end()
        {
            var existingItem = new PipelineItem<Action<string>>("item1", s => { });
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            pipeline.AddItemToEndOfPipeline(existingItem);
            pipeline.AddItemToEndOfPipeline(item2);

            pipeline.AddItemToEndOfPipeline(item1, true);

            Assert.Equal(2, pipeline.Items.Count());
            Assert.Same(item1, pipeline.Items.First());
        }

        [Fact]
        public void Should_replace_in_place_if_set_when_adding_at_index()
        {
            var existingItem = new PipelineItem<Action<string>>("item1", s => { });
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            pipeline.AddItemToEndOfPipeline(item2);
            pipeline.AddItemToEndOfPipeline(existingItem);

            pipeline.InsertItemAtPipelineIndex(0, item1, true);

            Assert.Equal(2, pipeline.Items.Count());
            Assert.Same(item1, pipeline.Items.Last());
        }

        [Fact]
        public void Should_be_able_to_remove_a_named_item()
        {
            var item1 = new PipelineItem<Action<string>>("item1", s => { });
            var item2 = new PipelineItem<Action<string>>("item2", s => { });
            pipeline.AddItemToEndOfPipeline(item1);
            pipeline.AddItemToEndOfPipeline(item2);

            pipeline.RemoveByName("item2");

            Assert.Equal(1, pipeline.Items.Count());
            Assert.Same(item1, pipeline.Items.First());
        }
    }
}