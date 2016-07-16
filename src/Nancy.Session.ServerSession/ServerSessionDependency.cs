using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nancy.Session.ServerSession
{
    public class ServerSessionDependency : ISessionDependency
    {
        public Func<IReadOnlyCollection<object>> GetInstancedDependencies
        {
            get
            {
                return () => { 
                    return (new object[] { }).ToList().AsReadOnly();
                };
            }            
        }

        public Func<IReadOnlyCollection<Type>> GetTypeDependencies
        {
            get
            {
                return () => { 
                    return (new Type[] { typeof(DefaultCookieConfiguration) }).ToList().AsReadOnly();
                };
            }
        }
    }
}
