namespace Nancy.Prototype.Registration
{
    using System.Collections.Generic;

    public interface IContainerRegistry
    {
        IReadOnlyCollection<TypeRegistration> TypeRegistrations { get; }

        IReadOnlyCollection<InstanceRegistration> InstanceRegistrations { get; }

        IReadOnlyCollection<CollectionTypeRegistration> CollectionTypeRegistrations { get; }
    }
}
