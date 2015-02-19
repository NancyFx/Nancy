#if !__MonoCS__
namespace Nancy.Tests.Unit.Bootstrapper.Base
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Nancy.Bootstrapper;
    using Nancy.Routing;

    using Xunit;

    /// <summary>
    /// Base class for testing the basic behaviour of a bootstrapper that
    /// implements either of the two bootstrapper base classes.
    /// These tests only test basic external behaviour, they are not exhaustive;
    /// it is expected that additional tests specific to the bootstrapper implementation
    /// are also created.
    /// </summary>
    public abstract class BootstrapperBaseFixtureBase<TContainer>
        where TContainer : class
    {
        private readonly NancyInternalConfiguration configuration;

        protected abstract NancyBootstrapperBase<TContainer> Bootstrapper { get; }

        protected NancyInternalConfiguration Configuration
        {
            get { return this.configuration; }
        }

        protected BootstrapperBaseFixtureBase()
        {
            this.configuration = NancyInternalConfiguration.WithOverrides(
                builder =>
                {
                    builder.NancyEngine = typeof(FakeEngine);
                });
        }

        [Fact]
        public virtual void Should_throw_if_get_engine_called_without_being_initialised()
        {
            // Given / When
            var result = Record.Exception(() => this.Bootstrapper.GetEngine());

            result.ShouldNotBeNull();
        }

        [Fact]
        public virtual void Should_resolve_engine_when_initialised()
        {
            // Given
            this.Bootstrapper.Initialise();

            // When
            var result = this.Bootstrapper.GetEngine();

            // Then
            result.ShouldNotBeNull();
            result.ShouldBeOfType(typeof(INancyEngine));
        }

        [Fact]
        public virtual void Should_use_types_from_config()
        {
            // Given
            this.Bootstrapper.Initialise();

            // When
            var result = this.Bootstrapper.GetEngine();

            // Then
            result.ShouldBeOfType(typeof(FakeEngine));
        }

        [Fact]
        public virtual void Should_register_config_types_as_singletons()
        {
            // Given
            this.Bootstrapper.Initialise();

            // When
            var result1 = this.Bootstrapper.GetEngine();
            var result2 = this.Bootstrapper.GetEngine();
            
            // Then
            result1.ShouldBeSameAs(result2);
        }

        [Fact]
        public void Should_honour_typeregistration_singleton_lifetimes()
        {
            // Given
            this.Bootstrapper.Initialise();
            
            // When
            var result1 = ((TestDependencyModule)this.Bootstrapper.GetModule(typeof(TestDependencyModule), new NancyContext()));
            var result2 = ((TestDependencyModule)this.Bootstrapper.GetModule(typeof(TestDependencyModule), new NancyContext()));

            // Then
            result1.Singleton.ShouldBeSameAs(result2.Singleton);
        }

        [Fact]
        public void Should_honour_typeregistration_transient_lifetimes()
        {
            // Given
            this.Bootstrapper.Initialise();

            // When
            var result1 = ((TestDependencyModule)this.Bootstrapper.GetModule(typeof(TestDependencyModule), new NancyContext()));
            var result2 = ((TestDependencyModule)this.Bootstrapper.GetModule(typeof(TestDependencyModule), new NancyContext()));

            // Then
            result1.Transient.ShouldNotBeSameAs(result2.Transient);
        }

        public class FakeEngine : INancyEngine
        {
            private readonly IRouteResolver resolver;
            private readonly IRouteCache routeCache;
            private readonly INancyContextFactory contextFactory;

            public INancyContextFactory ContextFactory
            {
                get { return this.contextFactory; }
            }

            public IRouteCache RouteCache
            {
                get { return this.routeCache; }
            }

            public IRouteResolver Resolver
            {
                get { return this.resolver; }
            }

            public Func<NancyContext, Response> PreRequestHook { get; set; }

            public Action<NancyContext> PostRequestHook { get; set; }

            public Func<NancyContext, Exception, dynamic> OnErrorHook { get; set; }

            public Func<NancyContext, IPipelines> RequestPipelinesFactory { get; set; }

            public Task<NancyContext> HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public FakeEngine(IRouteResolver resolver, IRouteCache routeCache, INancyContextFactory contextFactory)
            {
                if (resolver == null)
                {
                    throw new ArgumentNullException("resolver", "The resolver parameter cannot be null.");
                }

                if (routeCache == null)
                {
                    throw new ArgumentNullException("routeCache", "The routeCache parameter cannot be null.");
                }

                if (contextFactory == null)
                {
                    throw new ArgumentNullException("contextFactory");
                }

                this.resolver = resolver;
                this.routeCache = routeCache;
                this.contextFactory = contextFactory;
            }

            public void Dispose()
            {}
        }
    }

    public class TestRegistrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations { get; private set; }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; private set; }

        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }

        public TestRegistrations()
        {
            this.TypeRegistrations = new[]
                                         {
                                             new TypeRegistration(
                                                 typeof(Singleton),
                                                 typeof(Singleton),
                                                 Lifetime.Singleton),
                                             new TypeRegistration(
                                                 typeof(Transient),
                                                 typeof(Transient),
                                                 Lifetime.Transient),
                                         };
        }
    }

    public class Singleton
    {
    }

    public class Transient
    {
    }

    public class TestDependencyModule : NancyModule
    {
        public Singleton Singleton { get; set; }

        public Transient Transient { get; set; }

        public TestDependencyModule(Singleton singleton, Transient transient)
        {
            this.Singleton = singleton;
            this.Transient = transient;
        }
    }
}
#endif