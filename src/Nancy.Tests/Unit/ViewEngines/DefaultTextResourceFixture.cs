namespace Nancy.Tests.Unit.ViewEngines
{
    using Nancy.ViewEngines;
    using Xunit;

    public class DefaultTextResourceFixture
    {
        [Fact]
        public void Should_Return_Null_If_No_Assembly_Found()
        {
            //Given
            var defaultTextResource = new DefaultTextResource();
            var context = new NancyContext();

            //When
            var result = defaultTextResource["Greeting", context];

            //Then
            result.ShouldBeNull();
        }
    }
}
