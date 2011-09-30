namespace Nancy.Tests.Fakes
{
    using Nancy.Bootstrapper;

    public class MockApplicationPipelines : IApplicationPipelines
    {
        public BeforePipeline BeforeRequest { get; set; }

        public AfterPipeline AfterRequest { get; set; }

        public MockApplicationPipelines()
        {
            this.BeforeRequest = new BeforePipeline();
            this.AfterRequest = new AfterPipeline();
        }
    }

}