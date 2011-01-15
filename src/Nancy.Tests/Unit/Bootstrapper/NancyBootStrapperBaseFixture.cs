using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.BootStrapper;
using FakeItEasy;
using Xunit;

namespace Nancy.Tests.Unit.Bootstrapper
{
    public class FakeBootStrapperBaseImplementation : NancyBootStrapperBase<object>
    {
        public INancyEngine FakeNancyEngine { get; set; }
        public object FakeContainer { get;set; }
        public object AppContainer { get; set; }
        public List<ModuleRegistration> Modules { get; set; }
        public IModuleKeyGenerator Generator {get;set;}

        /// <summary>
        /// Initializes a new instance of the TestBootStrapper class.
        /// </summary>
        public FakeBootStrapperBaseImplementation()
        {
            FakeNancyEngine = A.Fake<INancyEngine>();
            FakeContainer = new object();
        }

        protected override IModuleKeyGenerator GetModuleKeyGenerator()
        {
            Generator = A.Fake<IModuleKeyGenerator>();
            A.CallTo(() => Generator.GetKeyForModuleType(A<Type>.Ignored)).Returns("FAKEMODULEKEYGENERATOR");
            return Generator;
        }

        protected override INancyEngine GetEngineInternal()
        {
            return FakeNancyEngine;
        }

        protected override object CreateContainer()
        {
            return FakeContainer;
        }

        protected override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            Modules = new List<ModuleRegistration>(moduleRegistrationTypes);
        }

        protected override void ConfigureApplicationContainer(object container)
        {
            base.ConfigureApplicationContainer(container);
            AppContainer = container;
        }
    }

    public class FakeBootStrapperBaseGetModulesOverride : NancyBootStrapperBase<object>
    {
        public bool GetModuleTypesCalled { get; set; }
        public IEnumerable<ModuleRegistration> RegisterModulesRegistrationTypes { get; set; }
        public IEnumerable<ModuleRegistration> ModuleRegistrations { get; set; }

        /// <summary>
        /// Initializes a new instance of the FakeBootStrapperBaseGetModulesOverride class.
        /// </summary>
        public FakeBootStrapperBaseGetModulesOverride()
        {
            ModuleRegistrations = new List<ModuleRegistration>() { new ModuleRegistration(this.GetType(), "FakeBootStrapperBaseGetModulesOverride") };
        }

        protected override IEnumerable<ModuleRegistration> GetModuleTypes(IModuleKeyGenerator moduleKeyGenerator)
        {
            return ModuleRegistrations;
        }

        protected override void RegisterModules(IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            RegisterModulesRegistrationTypes = moduleRegistrationTypes;
        }

        protected override INancyEngine GetEngineInternal()
        {
            return A.Fake<INancyEngine>();
        }

        protected override object CreateContainer()
        {
            return new object();
        }
    }

    public class NancyBootStrapperBaseFixture
    {
        private FakeBootStrapperBaseImplementation _BootStrapper;

        /// <summary>
        /// Initializes a new instance of the NancyBootStrapperBaseFixture class.
        /// </summary>
        public NancyBootStrapperBaseFixture()
        {
            _BootStrapper = new FakeBootStrapperBaseImplementation();
        }

        [Fact]
        public void GetEngine_Returns_Engine_From_GetEngineInternal()
        {
            var result = _BootStrapper.GetEngine();

            result.ShouldBeSameAs(_BootStrapper.FakeNancyEngine);
        }

        [Fact]
        public void GetEngine_Calls_ConfigureApplicationContainer_With_Container_From_GetContainer()
        {
            _BootStrapper.GetEngine();

            _BootStrapper.AppContainer.ShouldBeSameAs(_BootStrapper.FakeContainer);
        }

        [Fact]
        public void GetEngine_Calls_RegisterModules_With_Assembly_Modules()
        {
            _BootStrapper.GetEngine();

            _BootStrapper.Modules.ShouldNotBeNull();
            _BootStrapper.Modules.Where(mr => mr.ModuleType == typeof(Fakes.FakeNancyModuleWithBasePath)).FirstOrDefault().ShouldNotBeNull();
            _BootStrapper.Modules.Where(mr => mr.ModuleType == typeof(Fakes.FakeNancyModuleWithoutBasePath)).FirstOrDefault().ShouldNotBeNull();
        }

        [Fact]
        public void GetEngine_Gets_ModuleRegistration_Keys_For_Each_Module_From_IModuleKeyGenerator_From_GetModuleKeyGenerator()
        {
            _BootStrapper.GetEngine();

            var totalKeyEntries = _BootStrapper.Modules.Count();

            A.CallTo(() => _BootStrapper.Generator.GetKeyForModuleType(A<Type>.Ignored)).MustHaveHappened(Repeated.Times(totalKeyEntries).Exactly);
        }

        [Fact]
        public void Overridden_GetModules_Is_Used_For_Getting_ModuleTypes()
        {
            var bootstrapper = new FakeBootStrapperBaseGetModulesOverride();
            bootstrapper.GetEngine();

            bootstrapper.RegisterModulesRegistrationTypes.ShouldBeSameAs(bootstrapper.ModuleRegistrations);
        }
    }
}
