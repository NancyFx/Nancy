namespace Nancy.Tests.Fakes
{
    public interface IFoo
    {
    }

    public class Foo : IFoo
    {
    }

    public interface IDependency
    {
        IFoo FooDependency { get; set; }
    }

    public class Dependency : IDependency
    {
        public IFoo FooDependency { get; set; }

        /// <summary>
        /// Initializes a new instance of the Dependency class.
        /// </summary>
        public Dependency(IFoo fooDependency)
        {
            FooDependency = fooDependency;
        }
    }

    public class FakeNancyModuleWithDependency : NancyModule
    {
        public IDependency Dependency { get; set; }
        public IFoo FooDependency { get; set; }

        /// <summary>
        /// Initializes a new instance of the FakeNancyModuleWithDependency class.
        /// </summary>
        public FakeNancyModuleWithDependency(IDependency dependency, IFoo foo)
        {
            Dependency = dependency;
            FooDependency = foo;
        }
    }
}
