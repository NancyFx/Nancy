namespace Nancy.Tests.Fakes
{
    using System;

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
            : base("GET", "/", null, (x,c) => response)
        {
            this.Action = (x,c) => {
                this.ParametersUsedToInvokeAction = x;
                this.ActionWasInvoked = true;
                return response;
            };
        }

        public FakeRoute(Func<dynamic, dynamic> actionDelegate)
            : base("GET", "/", null, actionDelegate)
        {
            
        }
    }
}