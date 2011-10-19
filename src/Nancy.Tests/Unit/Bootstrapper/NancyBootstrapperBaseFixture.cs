namespace Nancy.Tests.Unit.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using FakeItEasy;

    using Nancy.Bootstrapper;
    using Nancy.Tests.Fakes;

    using Xunit;

    internal class FakeBootstrapperBaseImplementation : NancyBootstrapperBase<object>
    {
        public INancyEngine FakeNancyEngine { get; set; }
        public object FakeContainer { get; set; }
        public object AppContainer { get; set; }
        public IModuleKeyGenerator Generator { get; set; }
        public IEnumerable<TypeRegistration> TypeRegistrations { get; set; }
        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations { get; set; }
        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; set; }
        public List<ModuleRegistration> PassedModules { get; set; }
        public IStartup[] OverriddenStartupTasks { get; set; }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(c => c.ModuleKeyGenerator = typeof(FakeModuleKeyGenerator));
            }
        }

        public FakeBootstrapperBaseImplementation()
        {
            FakeNancyEngine = A.Fake<INancyEngine>();
            FakeContainer = new object();

            Generator = new Fakes.FakeModuleKeyGenerator();
        }

        protected override INancyEngine GetEngineInternal()
        {
            return this.FakeNancyEngine;
        }

        protected override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return this.Generator;
        }

        /// <summary>
        /// Gets all registered startup tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IStartup"/> instances. </returns>
        protected override IEnumerable<IStartup> GetStartupTasks()
        {
            return this.OverriddenStartupTasks ?? new IStartup[] { };
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        public override IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            return this.PassedModules.Select(m => (NancyModule)Activator.CreateInstance(m.ModuleType));
        }

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        public override NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            return
                this.PassedModules.Where(m => String.Equals(m.ModuleKey, moduleKey, StringComparison.InvariantCulture))
                    .Select(m => (NancyModule)Activator.CreateInstance(m.ModuleType))
                    .FirstOrDefault();
        }

        protected override void ConfigureApplicationContainer(object existingContainer)
        {
            this.AppContainer = existingContainer;
        }

        protected override object GetApplicationContainer()
        {
            return FakeContainer;
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
            this.TypeRegistrations = typeRegistrations;
        }

        protected override void RegisterCollectionTypes(object container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrations)
        {
            this.CollectionTypeRegistrations = collectionTypeRegistrations;
        }

        protected override void RegisterModules(object container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            PassedModules = new List<ModuleRegistration>(moduleRegistrationTypes);
        }

        protected override void RegisterInstances(object container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            this.InstanceRegistrations = instanceRegistrations;
        }

        protected override byte[] DefaultFavIcon
        {
            get { return this.Favicon ?? base.DefaultFavIcon; }
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
            ModuleRegistrations = new List<ModuleRegistration>() { new ModuleRegistration(this.GetType(), "FakeBootstrapperBaseGetModulesOverride") };
        }

        /// <summary>
        /// Gets all registered startup tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IStartup"/> instances. </returns>
        protected override IEnumerable<IStartup> GetStartupTasks()
        {
            return new IStartup[] { };
        }

        /// <summary>
        /// Get all NancyModule implementation instances
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        public override IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        public override NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            throw new NotImplementedException();
        }

        protected override INancyEngine GetEngineInternal()
        {
            return A.Fake<INancyEngine>();
        }

        protected override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            return new Fakes.FakeModuleKeyGenerator();
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

    public class NancyBootstrapperBaseFixture
    {
        private FakeBootstrapperBaseImplementation bootstrapper;

        /// <summary>
        /// Initializes a new instance of the NancyBootstrapperBaseFixture class.
        /// </summary>
        public NancyBootstrapperBaseFixture()
        {
            bootstrapper = new FakeBootstrapperBaseImplementation();
            bootstrapper.Initialise();
        }

        [Fact]
        public void GetEngine_Returns_Engine_From_GetEngineInternal()
        {
            var result = bootstrapper.GetEngine();

            result.ShouldBeSameAs(bootstrapper.FakeNancyEngine);
        }

        [Fact]
        public void GetEngine_Calls_ConfigureApplicationContainer_With_Container_From_GetContainer()
        {
            bootstrapper.GetEngine();

            bootstrapper.AppContainer.ShouldBeSameAs(bootstrapper.FakeContainer);
        }

        [Fact]
        public void GetEngine_Calls_RegisterModules_With_Assembly_Modules()
        {
            bootstrapper.GetEngine();

            bootstrapper.PassedModules.ShouldNotBeNull();
            bootstrapper.PassedModules.Where(mr => mr.ModuleType == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault().ShouldNotBeNull();
            bootstrapper.PassedModules.Where(mr => mr.ModuleType == typeof(Fakes.FakeNancyModuleWithoutBasePath)).FirstOrDefault().ShouldNotBeNull();
        }

        [Fact]
        public void GetEngine_Gets_ModuleRegistration_Keys_For_Each_Module_From_IModuleKeyGenerator_From_GetModuleKeyGenerator()
        {
            bootstrapper.GetEngine();

            var totalKeyEntries = bootstrapper.PassedModules.Count();
            var called = (bootstrapper.Generator as Fakes.FakeModuleKeyGenerator).CallCount;

            called.ShouldEqual(totalKeyEntries);
        }

        [Fact]
        public void Overridden_Modules_Is_Used_For_Getting_ModuleTypes()
        {
            var bootstrapper = new FakeBootstrapperBaseGetModulesOverride();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();

            bootstrapper.RegisterModulesRegistrationTypes.ShouldBeSameAs(bootstrapper.ModuleRegistrations);
        }

        [Fact]
        public void RegisterTypes_Passes_In_User_Types_If_Custom_Config_Set()
        {
            // Given
            bootstrapper.GetEngine();

            // When
            var moduleKeyGeneratorEntry = bootstrapper.TypeRegistrations.Where(tr => tr.RegistrationType == typeof(IModuleKeyGenerator)).FirstOrDefault();

            // Then
            moduleKeyGeneratorEntry.ImplementationType.ShouldEqual(typeof(Fakes.FakeModuleKeyGenerator));
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
            var startupMock = A.Fake<IStartup>();
            var startupMock2 = A.Fake<IStartup>();
            bootstrapper.OverriddenStartupTasks = new[] { startupMock, startupMock2 };

            // When
            bootstrapper.Initialise();

            // Then
            A.CallTo(() => startupMock.Initialize(A<IPipelines>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => startupMock2.Initialize(A<IPipelines>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_register_startup_task_type_registrations_into_container()
        {
            var typeRegistrations = new TypeRegistration[] { };
            var startupStub = A.Fake<IStartup>();
            A.CallTo(() => startupStub.TypeRegistrations).Returns(typeRegistrations);
            bootstrapper.OverriddenStartupTasks = new[] { startupStub };

            bootstrapper.Initialise();

            bootstrapper.TypeRegistrations.ShouldBeSameAs(typeRegistrations);
        }

        [Fact]
        public void Should_register_startup_task_collection_registrations_into_container()
        {
            var collectionTypeRegistrations = new CollectionTypeRegistration[] { };
            var startupStub = A.Fake<IStartup>();
            A.CallTo(() => startupStub.CollectionTypeRegistrations).Returns(collectionTypeRegistrations);
            bootstrapper.OverriddenStartupTasks = new[] { startupStub };

            bootstrapper.Initialise();

            bootstrapper.CollectionTypeRegistrations.ShouldBeSameAs(collectionTypeRegistrations);
        }

        [Fact]
        public void Should_register_startup_task_instance_registrations_into_container()
        {
            var instanceRegistrations = new InstanceRegistration[] { };
            var startupStub = A.Fake<IStartup>();
            A.CallTo(() => startupStub.InstanceRegistrations).Returns(instanceRegistrations);
            bootstrapper.OverriddenStartupTasks = new[] { startupStub };

            bootstrapper.Initialise();

            bootstrapper.InstanceRegistrations.ShouldBeSameAs(instanceRegistrations);
        }

        [Fact]
        public void Should_allow_favicon_override()
        {
            var favicon = new byte[] { 1, 2, 3 };
            bootstrapper.Favicon = favicon;
            var favIconRequest = new FakeRequest("GET", "/favicon.ico");
            var context = new NancyContext { Request = favIconRequest };
            bootstrapper.Initialise();

            var result = bootstrapper.PreRequest.Invoke(context);

            result.ShouldNotBeNull();
            result.ContentType.ShouldEqual("image/vnd.microsoft.icon");
            result.StatusCode = HttpStatusCode.OK;
            GetBodyBytes(result).SequenceEqual(favicon).ShouldBeTrue();
        }

        private byte[] GetBodyBytes(Response response)
        {
            using (var contentsStream = new MemoryStream())
            {
                response.Contents.Invoke(contentsStream);

                return contentsStream.ToArray();
            }
        }
    }
}
