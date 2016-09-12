namespace Nancy
{
    using Nancy.Session;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Default Dependencies for <see cref="DefaultCookieSessionProvider"/> 
    /// </summary>
    public class DefaultCookieSessionDependency : ISessionDependency
    {
        /// <summary>
        /// Returns instanced dependency <see cref="CookieBasedSessionsConfiguration"/> initialised with <see cref="DefaultObjectSerializer"/>
        /// </summary>
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

        /// <summary>
        /// Returns the dependecy of type <see cref="DefaultCookieConfiguration"/>
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
