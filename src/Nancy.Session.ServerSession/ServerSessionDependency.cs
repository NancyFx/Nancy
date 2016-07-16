using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nancy.Session.ServerSession
{
    /// <summary>
    /// Dependencies for <see cref="ServerSessionProvider"/> 
    /// </summary>
    public class ServerSessionDependency : ISessionDependency
    {
        /// <summary>
        /// Returns empty list
        /// </summary>
        public Func<IReadOnlyCollection<object>> GetInstancedDependencies
        {
            get
            {
                return () => { 
                    return (new object[] { }).ToList().AsReadOnly();
                };
            }            
        }

        /// <summary>
        /// Returns dependant type <see cref="DefaultCookieConfiguration"/> 
        /// </summary>
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
