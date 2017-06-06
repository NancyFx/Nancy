namespace Nancy.Prototype.Scanning
{
    using System;
    using System.Collections.Generic;

    public interface ITypeCatalog
    {
        IEnumerable<Type> GetTypesAssignableTo(Type type, ScanningStrategy strategy);
    }
}
