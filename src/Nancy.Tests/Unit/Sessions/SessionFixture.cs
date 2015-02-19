namespace Nancy.Tests
{
    using System.Collections.Generic;

    using Nancy.Session;

    using Xunit;

    public class SessionFixture
    {
        [Fact]
        public void Clearing_the_session_should_not_mark_it_as_changed_if_it_as_empty()
        {
            // Given
            var session = new Session();

            // When
            session.DeleteAll();

            // Then
            session.HasChanged.ShouldBeFalse();
        }

        [Fact]
        public void Clearing_the_session_should_mark_it_as_changed_if_it_was_not_empty()
        {
            // Given
            var session = new Session(new Dictionary<string, object> { { "key", 1 } });

            // When
            session.DeleteAll();

            // Then
            session.HasChanged.ShouldBeTrue();
        }

        [Fact]
        public void Deleting_an_invalid_key_should_not_mark_it_as_changed()
        {
            // Given
            var session = new Session(new Dictionary<string, object> { { "key", 1 } });

            // When
            session.Delete("something");

            // Then
            session.HasChanged.ShouldBeFalse();
        }

        [Fact]
        public void Deleting_a_key_should_mark_it_as_changed()
        {
            // Given
            var session = new Session(new Dictionary<string, object> { { "key", 1 } });

            // When
            session.Delete("key");

            // Then
            session.HasChanged.ShouldBeTrue();
        }

        [Fact]
        public void Setting_a_session_should_mark_it_as_changed()
        {
            // Given
            var session = new Session();

            // When
            session["key"] = "SomeValue";

            // Then
            session.HasChanged.ShouldBeTrue();
        }

        [Fact]
        public void Setting_a_key_with_the_same_value_should_not_mark_it_as_changed()
        {
          // Given
          var session = new Session(new Dictionary<string, object> { { "key", "SomeValue" } } );

          // When
          session["key"] = "SomeValue";

          // Then
          session.HasChanged.ShouldBeFalse();
        }

        [Fact]
        public void Setting_a_key_with_a_different_value_should_mark_it_as_changed()
        {
          // Given
          var session = new Session(new Dictionary<string, object> { { "key", "SomeValue" } });

          // When
          session["key"] = "SomeValue2";

          // Then
          session.HasChanged.ShouldBeTrue();
        }
    }
}