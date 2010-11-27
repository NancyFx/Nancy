namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Routing;
    using Xunit;

    public class RouteParametersFixture
    {
        [Fact]
        public void Should_support_dynamic_properties()
        {
            //Given
            dynamic parameters = new RouteParameters();
            parameters.test = 10;

            // When
            var value = (int)parameters.test;

            // Then
            value.ShouldEqual(10);
        }
    }
}