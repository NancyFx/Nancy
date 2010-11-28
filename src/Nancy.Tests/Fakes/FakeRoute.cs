namespace Nancy.Tests.Fakes
{
    using Nancy.Routing;

    public class FakeRoute : Route
    {
        public bool ActionWasInvoked;

        public FakeRoute() 
            : base(string.Empty, null, x => new Response())
        {
            this.Action = x => {
                this.ActionWasInvoked = true;
                return new Response();
            };
        }
    }
}