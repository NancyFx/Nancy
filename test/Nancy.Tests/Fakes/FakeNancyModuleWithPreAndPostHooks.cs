namespace Nancy.Tests.Fakes
{
    public class FakeNancyModuleWithPreAndPostHooks : NancyModule
    {
        public FakeNancyModuleWithPreAndPostHooks()
        {
            this.Before += (c) => null;
            this.After += (c) => { };

            Get("/PrePost", args =>
            {
                return new Response();
            });
        }
    }
}