using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nancy.Session
{
    public interface ISessionDependency
    {
        Func<IReadOnlyCollection<Type>> GetTypeDependencies { get; }

        Func<IReadOnlyCollection<object>> GetInstancedDependencies { get; }
    }
}