namespace Nancy.Tests.Unit.Localization
{
    using System;
    using FakeItEasy;

    using Nancy.Localization;

    using Xunit;

    public class ResourceBasedTextResourceFixture
    {
        [Fact]
        public void Should_Return_Null_If_No_Assembly_Found()
        {
            // Given
            var resourceAssemblyProvider = A.Fake<IResourceAssemblyProvider>();
            A.CallTo(() => resourceAssemblyProvider.GetAssembliesToScan()).Returns(new[] { typeof(NancyEngine).Assembly });

            var defaultTextResource = new ResourceBasedTextResource(resourceAssemblyProvider);
            var context = new NancyContext();

            // When
            var result = defaultTextResource["Texts.Greeting", context];

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_resource_string_if_unique_match_could_be_made()
        {
            // Given
            var resourceAssemblyProvider = A.Fake<IResourceAssemblyProvider>();
            A.CallTo(() => resourceAssemblyProvider.GetAssembliesToScan()).Returns(new[] { this.GetType().Assembly });

            var defaultTextResource = new ResourceBasedTextResource(resourceAssemblyProvider);
            var context = new NancyContext();

            // When
            var result = defaultTextResource["Menu.Home", context];

            // Then
            result.ShouldEqual("This is the home link");
        }

        [Fact]
        public void Should_throw_exception_when_multiple_resources_matches_key()
        {
            // Given
            const string expectedMessage = 
                "More than one text resources match the Texts key. Try providing a more specific key.";

            var resourceAssemblyProvider = A.Fake<IResourceAssemblyProvider>();
            A.CallTo(() => resourceAssemblyProvider.GetAssembliesToScan()).Returns(new[] { this.GetType().Assembly });

            var defaultTextResource = new ResourceBasedTextResource(resourceAssemblyProvider);
            var context = new NancyContext();

            // When
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                var result = defaultTextResource["Texts.Home", context];
            });

            // Then
            exception.Message.ShouldEqual(expectedMessage);
        }
    }
}
