namespace Nancy.Tests.Fakes
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

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
            : base("GET", "/", null, (parameters, token) => CreateResponseTask(response))
        {
            this.Action = (parameters, token) =>
            {
                this.ParametersUsedToInvokeAction = parameters;
                this.ActionWasInvoked = true;
                return CreateResponseTask(response);
            };
        }

        public FakeRoute(Func<dynamic, CancellationToken, Task<dynamic>> actionDelegate)
            : base("GET", "/", null, actionDelegate)
        {
            
        }

        private static Task<dynamic> CreateResponseTask(dynamic response)
        {
            var tcs = 
                new TaskCompletionSource<dynamic>();

            tcs.SetResult(response);

            return tcs.Task;
        }
    }
}