namespace Nancy.Tests.Fakes
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Nancy.Routing;

    public class FakeRoute : Route
    {
        private static Func<dynamic, CancellationToken, Task<dynamic>>DefaultAction = (parameters, token) => null;

        public bool ActionWasInvoked;
        public DynamicDictionary ParametersUsedToInvokeAction;

        public FakeRoute()
            : this(new Response())
        {
        }

        public FakeRoute(dynamic response)
            : base("GET", "/", null, (parametes, token) => null)
        {
            this.Action = Wrap(this, (parameters, token) => Task.FromResult<dynamic>(response));
        }

        public FakeRoute(Func<dynamic, CancellationToken, Task<dynamic>> action)
            : base("GET", "/", null, (parametes, token) => null)
        {
            this.Action = Wrap(this, action);
        }

        private static Func<dynamic, CancellationToken, Task<dynamic>> Wrap(
            FakeRoute route,
            Func<dynamic, CancellationToken, Task<dynamic>> action)
        {
            return (parameters, token) =>
            {
                route.ParametersUsedToInvokeAction = parameters;
                route.ActionWasInvoked = true;

                return action.Invoke(parameters, token);
            };
        }
    }
}