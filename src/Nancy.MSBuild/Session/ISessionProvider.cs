using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Session
{
    /// <summary>
    /// Interface for implementing Session
    /// </summary>
    public interface ISessionProvider
    {
        /// <summary>
        /// <para>Loads the session from the request.</para>
        /// </summary>
        /// <param name="context">Nancy Context</param>
        /// <returns>Session if found, otherwise returns null.</returns>
        ISession Load(NancyContext context);

        /// <summary>
        /// Save the session into the response
        /// </summary>
        /// <param name="context">Nancy Context</param>
        void Save(ISession session, NancyContext context);
        /// <summary>
        /// Initialise a new session
        /// </summary>
        /// <param name="context">Nancy Context</param>
        /// <returns></returns>
        ISession New(NancyContext context);

        /// <summary>
        /// Clear the session and delete the object
        /// </summary>
        /// <param name="context">Nancy Context</param>
        void Clear(NancyContext context);
    }
}
