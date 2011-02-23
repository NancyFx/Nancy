namespace Nancy.Tests.Fakes
{
    public class FakeNancyModuleWithPreAndPostHooks : NancyModule
    {
        public FakeNancyModuleWithPreAndPostHooks()
        {
            this.Before += (c) => null;
            this.After += (c) => { };

            Get["/PrePost"] = x =>
            {
                return new Response();
            };
        }
    }
}