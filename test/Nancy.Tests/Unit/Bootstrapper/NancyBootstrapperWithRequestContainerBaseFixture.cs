namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.Configuration;
    using Nancy.Diagnostics;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class NancyBootstrapperWithRequestContainerBaseFixture
    {
        private readonly FakeBootstrapper bootstrapper;

        public NancyBootstrapperWithRequestContainerBaseFixture()
        {
            this.bootstrapper = new FakeBootstrapper();
        }

        [Fact]
        public void Should_not_register_request_lifetime_types_into_application_container()
        {
            // Given
            var typeRegistrations = new[]
                                    {
                                        new TypeRegistration(typeof(object), typeof(object)),
                                        new TypeRegistration(typeof(FakeBootstrapperBaseImplementation), typeof(FakeBootstrapperBaseImplementation), Lifetime.Transient),
                                        new TypeRegistration(typeof(string), typeof(string), Lifetime.PerRequest),
                                    };
            var startupStub = A.Fake<IRegistrations>();
            A.CallTo(() => startupStub.TypeRegistrations).Returns(typeRegistrations);
            this.bootstrapper.OverriddenRegistrationTasks = new[] { startupStub };

            // When
            this.bootstrapper.Initialise();

            // Then
            this.bootstrapper.ApplicationTypeRegistrations.Any(tr => tr.RegistrationType == typeof(object)).ShouldBeTrue();
            this.bootstrapper.ApplicationTypeRegistrations.Any(tr => tr.RegistrationType == typeof(FakeBootstrapperBaseImplementation)).ShouldBeTrue();
            this.bootstrapper.ApplicationTypeRegistrations.Any(tr => tr.RegistrationType == typeof(string)).ShouldBeFalse();
        }

        [Fact]
        public void Should_register_request_lifetime_types_into_request_container_as_singletons()
        {
            // Given
            var typeRegistrations = new[]
                                    {
                                        new TypeRegistration(typeof(object), typeof(object)),
                                        new TypeRegistration(typeof(FakeBootstrapperBaseImplementation), typeof(FakeBootstrapperBaseImplementation), Lifetime.Transient),
                                        new TypeRegistration(typeof(string), typeof(string), Lifetime.PerRequest),
                                    };
            var startupStub = A.Fake<IRegistrations>();
            A.CallTo(() => startupStub.TypeRegistrations).Returns(typeRegistrations);
            var engine = new FakeEngine();
            this.bootstrapper.FakeNancyEngine = engine;
            this.bootstrapper.OverriddenRegistrationTasks = new[] { startupStub };
            this.bootstrapper.Initialise();

            // When
            var builtEngine = this.bootstrapper.GetEngine();
            builtEngine.HandleRequest(new FakeRequest("GET", "/"));

            // Then
            this.bootstrapper.RequestTypeRegistrations.Any(tr => tr.RegistrationType == typeof(string) && tr.Lifetime == Lifetime.Singleton).ShouldBeTrue();
        }

        [Fact]
        public void Should_not_register_request_lifetime_collectiontypes_into_application_container()
        {
            // Given
            var collectionRegistrations = new[]
                                    {
                                        new CollectionTypeRegistration(typeof(object), new[] { typeof(object) }),
                                        new CollectionTypeRegistration(typeof(FakeBootstrapperBaseImplementation), new[] { typeof(FakeBootstrapperBaseImplementation) }, Lifetime.Transient),
                                        new CollectionTypeRegistration(typeof(string), new[] { typeof(string) }, Lifetime.PerRequest),
                                    };
            var startupStub = A.Fake<IRegistrations>();
            A.CallTo(() => startupStub.CollectionTypeRegistrations).Returns(collectionRegistrations);
            this.bootstrapper.OverriddenRegistrationTasks = new[] { startupStub };

            // When
            this.bootstrapper.Initialise();

            // Then
            this.bootstrapper.ApplicationCollectionTypeRegistrations.Any(tr => tr.RegistrationType == typeof(object)).ShouldBeTrue();
            this.bootstrapper.ApplicationCollectionTypeRegistrations.Any(tr => tr.RegistrationType == typeof(FakeBootstrapperBaseImplementation)).ShouldBeTrue();
            this.bootstrapper.ApplicationCollectionTypeRegistrations.Any(tr => tr.RegistrationType == typeof(string)).ShouldBeFalse();
        }

        [Fact]
        public void Should_register_request_lifetime_collectiontypes_into_request_container_as_singletons()
        {
            // Given
            var collectionRegistrations = new[]
                                    {
                                        new CollectionTypeRegistration(typeof(object), new[] { typeof(object) }),
                                        new CollectionTypeRegistration(typeof(FakeBootstrapperBaseImplementation), new[] { typeof(FakeBootstrapperBaseImplementation) }, Lifetime.Transient),
                                        new CollectionTypeRegistration(typeof(string), new[] { typeof(string) }, Lifetime.PerRequest),
                                    };
            var startupStub = A.Fake<IRegistrations>();
            A.CallTo(() => startupStub.CollectionTypeRegistrations).Returns(collectionRegistrations);
            var engine = new FakeEngine();
            this.bootstrapper.FakeNancyEngine = engine;
            this.bootstrapper.OverriddenRegistrationTasks = new[] { startupStub };
            this.bootstrapper.Initialise();

            // When
            var builtEngine = this.bootstrapper.GetEngine();
            builtEngine.HandleRequest(new FakeRequest("GET", "/"));

            // Then
            this.bootstrapper.RequestCollectionTypeRegistrations.Any(tr => tr.RegistrationType == typeof(string) && tr.Lifetime == Lifetime.Singleton).ShouldBeTrue();
        }

        [Fact]
        public void Should_invoke_request_startup_tasks_when_request_pipelines_initialised()
        {
            // Given
            var startupMock = A.Fake<IRequestStartup>();
            var startupMock2 = A.Fake<IRequestStartup>();
            this.bootstrapper.RequestStartupTypes = new[] { typeof(object) };
            this.bootstrapper.OverriddenRequestStartupTasks = new[] { startupMock, startupMock2 };
            this.bootstrapper.Initialise();

            // When
            this.bootstrapper.GetRequestPipelines(new NancyContext());

            // Then
            A.CallTo(() => startupMock.Initialize(A<IPipelines>._, A<NancyContext>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => startupMock2.Initialize(A<IPipelines>._, A<NancyContext>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_resolve_request_startup_tasks_from_request_container()
        {
            // Given
            var startupMock = A.Fake<IRequestStartup>();
            this.bootstrapper.RequestStartupTypes = new[] { typeof(object) };
            this.bootstrapper.OverriddenRequestStartupTasks = new[] { startupMock };
            this.bootstrapper.Initialise();

            // When
            this.bootstrapper.GetRequestPipelines(new NancyContext());

            // Then
            this.bootstrapper.RequestStartupTasksResolveContainer.Parent.ShouldNotBeNull();
        }

        [Fact]
        public void Should_not_ask_to_resolve_request_startups_if_none_registered()
        {
            // Given
            this.bootstrapper.RequestStartupTypes = new Type[] { };
            this.bootstrapper.OverriddenRequestStartupTasks = new IRequestStartup[] { };
            this.bootstrapper.Initialise();

            // When
            this.bootstrapper.GetRequestPipelines(new NancyContext());

            // Then
            this.bootstrapper.GetRequestStartupTasksCalled.ShouldBeFalse();
        }

        internal class FakeEngine : INancyEngine
        {
            public Func<NancyContext, IPipelines> RequestPipelinesFactory { get; set; }

            public Task<NancyContext> HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest, CancellationToken cancellationToken)
            {
                var tcs = new TaskCompletionSource<NancyContext>();

                var nancyContext = new NancyContext() { Request = request };

                this.RequestPipelinesFactory.Invoke(nancyContext);

                tcs.SetResult(nancyContext);

                return tcs.Task;
            }

            public void Dispose()
            {
            }
        }

        internal class FakeBootstrapper : NancyBootstrapperWithRequestContainerBase<FakeContainer>
        {
            public IDiagnostics FakeDiagnostics { get; set; }

            public INancyEngine FakeNancyEngine { get; set; }

            public FakeContainer FakeContainer { get; set; }

            public FakeContainer AppContainer { get; set; }

            public IEnumerable<TypeRegistration> ApplicationTypeRegistrations { get; set; }

            public IEnumerable<TypeRegistration> RequestTypeRegistrations { get; set; }

            public IEnumerable<CollectionTypeRegistration> ApplicationCollectionTypeRegistrations { get; set; }

            public IEnumerable<CollectionTypeRegistration> RequestCollectionTypeRegistrations { get; set; }

            public IEnumerable<InstanceRegistration> InstanceRegistrations { get; set; }

            public List<ModuleRegistration> PassedModules { get; set; }

            public IApplicationStartup[] OverriddenApplicationStartupTasks { get; set; }

            public IRequestStartup[] OverriddenRequestStartupTasks { get; set; }

            public IRegistrations[] OverriddenRegistrationTasks { get; set; }

            public bool ShouldThrowWhenGettingEngine { get; set; }

            public FakeContainer RequestStartupTasksResolveContainer { get; set; }

            public bool GetRequestStartupTasksCalled { get; set; }

            public IEnumerable<Type> RequestStartupTypes { get; set; }

            public FakeBootstrapper()
            {
                FakeNancyEngine = A.Fake<INancyEngine>();
                FakeContainer = new FakeContainer();
            }

            protected override IEnumerable<Type> RequestStartupTasks
            {
                get
                {
                    return this.RequestStartupTypes ?? base.RequestStartupTasks;
                }
            }

            protected override INancyEngine GetEngineInternal()
            {
                if (this.ShouldThrowWhenGettingEngine)
                {
                    throw new ApplicationException("Something when wrong when trying to compose the engine.");
                }

                return this.FakeNancyEngine;
            }

            protected override INancyEnvironmentConfigurator GetEnvironmentConfigurator()
            {
                return new FakeEnvironmentConfigurator();
            }

            /// <summary>
            /// Gets the diagnostics for initialisation
            /// </summary>
            /// <returns>IDiagnostics implementation</returns>
            protected override IDiagnostics GetDiagnostics()
            {
                return this.FakeDiagnostics ?? new DisabledDiagnostics();
            }

            /// <summary>
            /// Gets all registered startup tasks
            /// </summary>
            /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances. </returns>
            protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
            {
                return this.OverriddenApplicationStartupTasks ?? new IApplicationStartup[] { };
            }

            protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(FakeContainer container, Type[] requestStartupTypes)
            {
                this.RequestStartupTasksResolveContainer = container;

                this.GetRequestStartupTasksCalled = true;

                return this.OverriddenRequestStartupTasks ?? new IRequestStartup[] { };
            }

            /// <summary>
            /// Gets all registered application registration tasks
            /// </summary>
            /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRegistrations"/> instances.</returns>
            protected override IEnumerable<IRegistrations> GetRegistrationTasks()
            {
                return this.OverriddenRegistrationTasks ?? new IRegistrations[] { };
            }

            public override INancyEnvironment GetEnvironment()
            {
                throw new NotImplementedException();
            }

            protected override void ConfigureApplicationContainer(FakeContainer existingContainer)
            {
                this.AppContainer = existingContainer;
            }

            protected override FakeContainer GetApplicationContainer()
            {
                return FakeContainer;
            }

            /// <summary>
            /// Registers an <see cref="INancyEnvironment"/> instance in the container.
            /// </summary>
            /// <param name="container">The container to register into.</param>
            /// <param name="environment">The <see cref="INancyEnvironment"/> instance to register.</param>
            protected override void RegisterNancyEnvironment(FakeContainer container, INancyEnvironment environment)
            {
            }

            /// <summary>
            /// Register the bootstrapper's implemented types into the container.
            /// This is necessary so a user can pass in a populated container but not have
            /// to take the responsibility of registering things like INancyModuleCatalog manually.
            /// </summary>
            /// <param name="applicationContainer">Application container to register into</param>
            protected override void RegisterBootstrapperTypes(FakeContainer applicationContainer)
            {
            }

            protected override void RegisterTypes(
                FakeContainer container,
                IEnumerable<TypeRegistration> typeRegistrations)
            {
                if (ReferenceEquals(container, this.AppContainer))
                {
                    this.ApplicationTypeRegistrations = typeRegistrations;
                }
                else
                {
                    this.RequestTypeRegistrations = typeRegistrations;
                }
            }

            protected override void RegisterCollectionTypes(
                FakeContainer container,
                IEnumerable<CollectionTypeRegistration> collectionTypeRegistrations)
            {
                if (ReferenceEquals(container, this.AppContainer))
                {
                    this.ApplicationCollectionTypeRegistrations = collectionTypeRegistrations;
                }
                else
                {
                    this.RequestCollectionTypeRegistrations = collectionTypeRegistrations;
                }
            }

            protected override FakeContainer CreateRequestContainer(NancyContext context)
            {
                return new FakeContainer(this.ApplicationContainer);
            }

            protected override void RegisterRequestContainerModules(FakeContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
            {
                PassedModules = new List<ModuleRegistration>(moduleRegistrationTypes);
            }

            protected override IEnumerable<INancyModule> GetAllModules(FakeContainer container)
            {
                return this.PassedModules.Select(m => (INancyModule)Activator.CreateInstance(m.ModuleType));
            }

            protected override INancyModule GetModule(FakeContainer container, Type moduleType)
            {
                return
                    this.PassedModules.Where(m => m.ModuleType == moduleType)
                        .Select(m => (INancyModule)Activator.CreateInstance(m.ModuleType))
                        .FirstOrDefault();
            }

            protected override void RegisterInstances(
                FakeContainer container,
                IEnumerable<InstanceRegistration> instanceRegistrations)
            {
                this.InstanceRegistrations = instanceRegistrations;
            }

            protected override byte[] FavIcon
            {
                get
                {
                    return this.Favicon ?? base.FavIcon;
                }
            }

            public BeforePipeline PreRequest
            {
                get
                {
                    return this.ApplicationPipelines.BeforeRequest;
                }
                set
                {
                    this.ApplicationPipelines.BeforeRequest = value;
                }
            }

            public AfterPipeline PostRequest
            {
                get
                {
                    return this.ApplicationPipelines.AfterRequest;
                }
                set
                {
                    this.ApplicationPipelines.AfterRequest = value;
                }
            }

            public byte[] Favicon { get; set; }

            public IPipelines GetRequestPipelines(NancyContext nancyContext)
            {
                return this.InitializeRequestPipelines(nancyContext);
            }
        }

        internal class FakeContainer : IDisposable
        {
            public FakeContainer Parent { get; private set; }

            public FakeContainer(FakeContainer parent = null)
            {
                this.Parent = parent;
            }

            public void Dispose()
            {
                this.Disposed = true;
            }

            public bool Disposed { get; private set; }
        }
    }
}