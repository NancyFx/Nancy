namespace Nancy.Tests
{
    using Nancy.Hosting.Owin;

    using Xunit;

    public class ResponseStreamFixture
    {
        [Fact]
        public void Should_be_idempotent_when_calling_dispose()
        {
            var stream = new ResponseStream((arr, act) => false, () => { });

            stream.Dispose();
            stream.Dispose();
        }
    }
}