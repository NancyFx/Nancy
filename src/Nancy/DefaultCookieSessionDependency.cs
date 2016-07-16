using Nancy.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nancy
{
    public class DefaultCookieSessionDependency : ISessionDependency
    {
        public Func<IReadOnlyCollection<object>> GetInstancedDependencies
        {
            get
            {
                return () => {
                    return (new object[] {
                            new CookieBasedSessionsConfiguration()
                            {
                                Serializer = new DefaultObjectSerializer()
                            }
                        }).ToList().AsReadOnly();
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
