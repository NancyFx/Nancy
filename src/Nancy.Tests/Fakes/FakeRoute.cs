namespace Nancy.Tests.Fakes
{
    using Nancy.Routing;

    public class FakeRoute : Route
    {
        public bool ActionWasInvoked;
        public DynamicDictionary ParametersUsedToInvokeAction;

        public FakeRoute()
            : this(new Response())
        {

        }

        public FakeRoute(dynamic response)
            : base("GET", "/", null, x => response)
        {
            this.Action = x => {
                this.ParametersUsedToInvokeAction = x;
                this.ActionWasInvoked = true;
                return response;
            };
        }
    }
}