namespace Nancy.Tests.Unit.ViewEngines
{
    using Xunit;
    using Nancy.Localization;

    public class DefaultTextResourceFixture
    {
        [Fact]
        public void Should_Return_Null_If_No_Assembly_Found()
        {
            //Given
            var defaultTextResource = new ResourceBasedTextResource();
            var context = new NancyContext();

            //When
            var result = defaultTextResource["Greeting", context];

            //Then
            result.ShouldBeNull();
        }
    }
}
