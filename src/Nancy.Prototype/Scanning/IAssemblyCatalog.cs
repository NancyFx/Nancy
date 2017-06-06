namespace Nancy.Prototype.Scanning
{
    using System.Collections.Generic;
    using System.Reflection;

    public interface IAssemblyCatalog
    {
        IEnumerable<Assembly> GetAssemblies();
    }
}
