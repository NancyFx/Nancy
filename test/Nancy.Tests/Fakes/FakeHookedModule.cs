namespace Nancy.Tests.Fakes
{
    public class FakeHookedModule : NancyModule
    {
        public FakeHookedModule(BeforePipeline before = null, AfterPipeline after = null)
        {
            this.Before = before;
            this.After = after;
        }
    }
}