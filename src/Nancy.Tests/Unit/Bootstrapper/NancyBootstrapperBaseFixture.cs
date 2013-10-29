using Nancy.Diagnostics;

namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Collections;
    using System.Reflection;
    using System.Threading;
    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class NancyBootstrapperBaseFixture
    {
        private readonly FakeBootstrapperBaseImplementation bootstrapper;

        /// <summary>
        /// Initializes a new instance of the NancyBootstrapperBaseFixture class.
        /// </summary>
        public NancyBootstrapperBaseFixture()
        {
            this.bootstrapper = new FakeBootstrapperBaseImplementation();
            this.bootstrapper.Initialise();
        }

        [Fact]
        public void GetEngine_Returns_Engine_From_GetEngineInternal()
        {
            // Given
            // When
            var result = this.bootstrapper.GetEngine();

            // Then
            result.ShouldBeSameAs(bootstrapper.FakeNancyEngine);
        }

        [Fact]
        public void Should_throw_invalidaoperationexception_when_get_engine_fails()
        {
            // Given
            this.bootstrapper.ShouldThrowWhenGettingEngine = true;

            // When
            var exception = Record.Exception(() => this.bootstrapper.GetEngine());

            // Then
            exception.ShouldBeOfType<InvalidOperationException>();
            exception.Message.ShouldEqual("Something went wrong when trying to satisfy one of the dependencies during composition, make sure that you've registered all new dependencies in the container and inspect the innerexception for more details.");
        }

        [Fact]
        public void GetEngine_Calls_ConfigureApplicationContainer_With_Container_From_GetContainer()
        {
            // Given
            // When
            this.bootstrapper.GetEngine();

            // Then
            this.bootstrapper.AppContainer.ShouldBeSameAs(bootstrapper.FakeContainer);
        }

        [Fact]
        public void GetEngine_Calls_RegisterModules_With_Assembly_Modules()
        {
            // Given
            // When
            this.bootstrapper.GetEngine();

            // Then
            this.bootstrapper.PassedModules.ShouldNotBeNull();
            this.bootstrapper.PassedModules.Where(mr => mr.ModuleType == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault().ShouldNotBeNull();
            this.bootstrapper.PassedModules.Where(mr => mr.ModuleType == typeof(Fakes.FakeNancyModuleWithoutBasePath)).FirstOrDefault().ShouldNotBeNull();
        }

        [Fact]
        public void Overridden_Modules_Is_Used_For_Getting_ModuleTypes()
        {
            // Given
            var localBootstrapper = new FakeBootstrapperBaseGetModulesOverride();

            // When
            localBootstrapper.Initialise();
            localBootstrapper.GetEngine();

            // Then
            localBootstrapper.RegisterModulesRegistrationTypes.ShouldBeSameAs(localBootstrapper.ModuleRegistrations);
        }

        [Fact]
        public void GetEngine_sets_request_pipelines_factory()
        {
            // Given
            this.bootstrapper.PreRequest += ctx => null;

            // When
            var result = this.bootstrapper.GetEngine();

            // Then
            result.RequestPipelinesFactory.ShouldNotBeNull();
        }

        [Fact]
        public void Should_invoke_startup_tasks()
        {
            // Given
            var startupMock = A.Fake<IApplicationStartup>();
            var startupMock2 = A.Fake<IApplicationStartup>();
            this.bootstrapper.OverriddenApplicationStartupTasks = new[] { startupMock, startupMock2 };

            // When
            this.bootstrapper.Initialise();

            // Then
            A.CallTo(() => startupMock.Initialize(A<IPipelines>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => startupMock2.Initialize(A<IPipelines>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_invoke_startup_tasks_after_registration_tasks()
        {
            // Given
            var startup = A.Fake<IApplicationStartup>();
            this.bootstrapper.OverriddenApplicationStartupTasks = new[] { startup };
            
            var registrations = A.Fake<IApplicationRegistrations>();
            this.bootstrapper.OverriddenApplicationRegistrationTasks = new[] { registrations };

            // When
            using(var scope = Fake.CreateScope())
            {
                this.bootstrapper.Initialise();

                // Then
                using (scope.OrderedAssertions())
                {
                    A.CallTo(() => registrations.CollectionTypeRegistrations).MustHaveHappened();
                    A.CallTo(() => startup.Initialize(A<IPipelines>._)).MustHaveHappened();
                }
            }
        }

        [Fact]
        public void Should_register_application_registration_type_registrations_into_container()
        {
            // Given
            var typeRegistrations = new TypeRegistration[] { };
            var startupStub = A.Fake<IApplicationRegistrations>();
            A.CallTo(() => startupStub.TypeRegistrations).Returns(typeRegistrations);
            this.bootstrapper.OverriddenApplicationRegistrationTasks = new[] { startupStub };

            // When
            this.bootstrapper.Initialise();

            // Then
            this.bootstrapper.TypeRegistrations.ShouldBeSameAs(typeRegistrations);
        }

        [Fact]
        public void Should_register_application_registration_task_collection_registrations_into_container()
        {
            // Given
            var collectionTypeRegistrations = new CollectionTypeRegistration[] { };
            var startupStub = A.Fake<IApplicationRegistrations>();
            A.CallTo(() => startupStub.CollectionTypeRegistrations).Returns(collectionTypeRegistrations);
            this.bootstrapper.OverriddenApplicationRegistrationTasks = new[] { startupStub };

            // When
            this.bootstrapper.Initialise();

            // Then
            this.bootstrapper.CollectionTypeRegistrations.ShouldBeSameAs(collectionTypeRegistrations);
        }

        [Fact]
        public void Should_register_application_registration_instance_registrations_into_container()
        {
            // Given
            var instanceRegistrations = new InstanceRegistration[] { };
            var startupStub = A.Fake<IApplicationRegistrations>();
            A.CallTo(() => startupStub.InstanceRegistrations).Returns(instanceRegistrations);
            this.bootstrapper.OverriddenApplicationRegistrationTasks = new[] { startupStub };

            // When
            this.bootstrapper.Initialise();

            // Then
            this.bootstrapper.InstanceRegistrations.ShouldBeSameAs(instanceRegistrations);
        }

        [Fact]
        public void Should_allow_favicon_override()
        {
            // Given
            var favicon = new byte[] { 1, 2, 3 };
            this.bootstrapper.Favicon = favicon;
            var favIconRequest = new FakeRequest("GET", "/favicon.ico");
            var context = new NancyContext { Request = favIconRequest };
            this.bootstrapper.Initialise();

            // When
            var result = this.bootstrapper.PreRequest.Invoke(context, new CancellationToken());

            // Then
            result.ShouldNotBeNull();
            result.Result.ContentType.ShouldEqual("image/vnd.microsoft.icon");
            result.Result.StatusCode = HttpStatusCode.OK;
            GetBodyBytes(result.Result).SequenceEqual(favicon).ShouldBeTrue();
        }

        [Fact]
        public void Should_get_diagnostics_and_initialise()
        {
            var fakeDiagnostics = A.Fake<IDiagnostics>();
            this.bootstrapper.FakeDiagnostics = fakeDiagnostics;

            this.bootstrapper.Initialise();

            A.CallTo(() => fakeDiagnostics.Initialize(A<IPipelines>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_dispose_app_container_if_bootstrapper_disposed()
        {
            this.bootstrapper.Initialise();
            this.bootstrapper.GetEngine();

            this.bootstrapper.Dispose();

            this.bootstrapper.AppContainer.Disposed.ShouldBeTrue();
        }

        private static IEnumerable<byte> GetBodyBytes(Response response)
        {
            using (var contentsStream = new MemoryStream())
            {
                response.Contents.Invoke(contentsStream);

                return contentsStream.ToArray();
            }
        }
    }

    internal class FakeBootstrapperBaseImplementation : NancyBootstrapperBase<FakeContainer>
    {
        public IDiagnostics FakeDiagnostics { get; set; }
        public INancyEngine FakeNancyEngine { get; set; }
        public FakeContainer FakeContainer { get; set; }
        public FakeContainer AppContainer { get; set; }
        public IEnumerable<TypeRegistration> TypeRegistrations { get; set; }
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; set; }
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; set; }
        public List<ModuleRegistration> PassedModules { get; set; }
        public IApplicationStartup[] OverriddenApplicationStartupTasks { get; set; }
        public IApplicationRegistrations[] OverriddenApplicationRegistrationTasks { get; set; }
        public bool ShouldThrowWhenGettingEngine { get; set; }

        public FakeBootstrapperBaseImplementation()
        {
            FakeNancyEngine = A.Fake<INancyEngine>();
            FakeContainer = new FakeContainer();
        }

        protected override INancyEngine GetEngineInternal()
        {
            if (this.ShouldThrowWhenGettingEngine)
            {
                throw new ApplicationException("Something when wrong when trying to compose the engine.");
            }

            return this.FakeNancyEngine;
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

        /// <summary>
        /// Gets all registered application registration tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationRegistrations"/> instances.</returns>
        protected override IEnumerable<IApplicationRegistrations> GetApplicationRegistrationTasks()
        {
            return this.OverriddenApplicationRegistrationTasks ?? new IApplicationRegistrations[] { };
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="INancyModule"/> instances.</returns>
        public override IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            return this.PassedModules.Select(m => (INancyModule)Activator.CreateInstance(m.ModuleType));
        }

        public override INancyModule GetModule(Type moduleType, NancyContext context)
        {
            return
                this.PassedModules.Where(m => m.ModuleType == moduleType)
                    .Select(m => (INancyModule)Activator.CreateInstance(m.ModuleType))
                    .FirstOrDefault();
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
        /// Register the bootstrapper's implemented types into the container.
        /// This is necessary so a user can pass in a populated container but not have
        /// to take the responsibility of registering things like INancyModuleCatalog manually.
        /// </summary>
        /// <param name="applicationContainer">Application container to register into</param>
        protected override void RegisterBootstrapperTypes(FakeContainer applicationContainer)
        {
        }

        protected override void RegisterTypes(FakeContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            this.TypeRegistrations = typeRegistrations;
        }

        protected override void RegisterCollectionTypes(FakeContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrations)
        {
            this.CollectionTypeRegistrations = collectionTypeRegistrations;
        }

        protected override void RegisterModules(FakeContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            PassedModules = new List<ModuleRegistration>(moduleRegistrationTypes);
        }

        protected override void RegisterInstances(FakeContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            this.InstanceRegistrations = instanceRegistrations;
        }

        protected override byte[] FavIcon
        {
            get { return this.Favicon ?? base.FavIcon; }
        }

        public BeforePipeline PreRequest
        {
            get { return this.ApplicationPipelines.BeforeRequest; }
            set { this.ApplicationPipelines.BeforeRequest = value; }
        }

        public AfterPipeline PostRequest
        {
            get { return this.ApplicationPipelines.AfterRequest; }
            set { this.ApplicationPipelines.AfterRequest = value; }
        }

        public byte[] Favicon { get; set; }
    }

    internal class FakeContainer : IDisposable
    {
        public void Dispose()
        {
            this.Disposed = true;
        }

        public bool Disposed { get; private set; }
    }

    internal class FakeBootstrapperBaseGetModulesOverride : NancyBootstrapperBase<object>
    {
        public IEnumerable<ModuleRegistration> RegisterModulesRegistrationTypes { get; set; }
        public IEnumerable<ModuleRegistration> ModuleRegistrations { get; set; }

        protected override IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                return ModuleRegistrations;
            }
        }

        public FakeBootstrapperBaseGetModulesOverride()
        {
            ModuleRegistrations = new List<ModuleRegistration>() { new ModuleRegistration(this.GetType()) };
        }

        /// <summary>
        /// Gets the diagnostics for initialisation
        /// </summary>
        /// <returns>IDiagnostics implementation</returns>
        protected override IDiagnostics GetDiagnostics()
        {
            return new DisabledDiagnostics();
        }

        /// <summary>
        /// Gets all registered startup tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances. </returns>
        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            return new IApplicationStartup[] { };
        }

        /// <summary>
        /// Gets all registered application registration tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationRegistrations"/> instances.</returns>
        protected override IEnumerable<IApplicationRegistrations> GetApplicationRegistrationTasks()
        {
            return new IApplicationRegistrations[] { };
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="INancyModule"/> instances.</returns>
        public override IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a specific <see cref="INancyModule"/> implementation - should be per-request lifetime
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="INancyModule"/> instance</returns>
        public override INancyModule GetModule(Type moduleType, NancyContext context)
        {
            throw new NotImplementedException();
        }

        protected override INancyEngine GetEngineInternal()
        {
            return A.Fake<INancyEngine>();
        }

        protected override object GetApplicationContainer()
        {
            return new object();
        }

        /// <summary>
        /// Register the bootstrapper's implemented types into the container.
        /// This is necessary so a user can pass in a populated container but not have
        /// to take the responsibility of registering things like INancyModuleCatalog manually.
        /// </summary>
        /// <param name="applicationContainer">Application container to register into</param>
        protected override void RegisterBootstrapperTypes(object applicationContainer)
        {
        }

        protected override void RegisterTypes(object container, IEnumerable<TypeRegistration> typeRegistrations)
        {
        }

        protected override void RegisterCollectionTypes(object container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrationsn)
        {
        }

        protected override void RegisterModules(object container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            this.RegisterModulesRegistrationTypes = moduleRegistrationTypes;
        }

        protected override void RegisterInstances(object container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
        }
    }
}
