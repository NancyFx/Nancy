namespace Nancy.Tests.Fakes
{
    using System;

    using Nancy.Routing;

    public class FakeRoutePatternMatchResult : IRoutePatternMatchResult
    {
        public FakeRoutePatternMatchResult(bool isMatch)
        {
            this.IsMatch = isMatch;
            this.Parameters = new DynamicDictionary();
        }

        public FakeRoutePatternMatchResult(Action<FakeRoutePatternMatchResultConfigurator> closure)
            : this(closure, new DynamicDictionary())
        {
        }

        public FakeRoutePatternMatchResult(Action<FakeRoutePatternMatchResultConfigurator> closure, DynamicDictionary parameters)
        {
            this.Parameters = parameters;

            var configurator =
                new FakeRoutePatternMatchResultConfigurator(this);

            closure.Invoke(configurator);
        }

        public NancyContext Context { get; private set; }

        public bool IsMatch { get; private set; }

        public DynamicDictionary Parameters { get; private set; }

        public class FakeRoutePatternMatchResultConfigurator
        {
            private readonly FakeRoutePatternMatchResult fakeRoutePatternMatchResult;

            public FakeRoutePatternMatchResultConfigurator(FakeRoutePatternMatchResult fakeRoutePatternMatchResult)
            {
                this.fakeRoutePatternMatchResult = fakeRoutePatternMatchResult;
            }

            public FakeRoutePatternMatchResultConfigurator Context(NancyContext context)
            {
                this.fakeRoutePatternMatchResult.Context = context;
                return this;
            }

            public FakeRoutePatternMatchResultConfigurator IsMatch(bool isMatch)
            {
                this.fakeRoutePatternMatchResult.IsMatch = isMatch;
                return this;
            }

            public FakeRoutePatternMatchResultConfigurator AddParameter(string name, object value)
            {
                this.fakeRoutePatternMatchResult.Parameters[name] = value;
                return this;
            }
        }
    }
}