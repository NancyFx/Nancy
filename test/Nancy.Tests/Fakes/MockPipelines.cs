namespace Nancy.Tests.Fakes
{
    using Nancy.Bootstrapper;

    public class MockPipelines : IPipelines
    {
        public BeforePipeline BeforeRequest { get; set; }

        public AfterPipeline AfterRequest { get; set; }

        public ErrorPipeline OnError { get; set; }

        public MockPipelines()
        {
            this.BeforeRequest = new BeforePipeline();
            this.AfterRequest = new AfterPipeline();
            this.OnError = new ErrorPipeline();
        }
    }

}