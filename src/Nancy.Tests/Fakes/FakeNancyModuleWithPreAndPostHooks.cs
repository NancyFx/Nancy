namespace Nancy.Tests.Fakes
{
    public class FakeNancyModuleWithPreAndPostHooks : NancyModule
    {
        public FakeNancyModuleWithPreAndPostHooks()
        {
            this.PreRequestHooks += (c) => null;
            this.PostRequestHooks += (c) => { };

            Get["/PrePost"] = x =>
            {
                return new Response();
            };
        }
    }
}