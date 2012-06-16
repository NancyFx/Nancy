using System;
using Nancy.Security;

namespace Nancy.Authentication.Stateless
{
    /// <summary>
    /// Configuration options for stateless authentication
    /// </summary>
    public class StatelessAuthenticationConfiguration
    {
        internal Func<NancyContext, IUserIdentity> GetUserIdentity;

        public StatelessAuthenticationConfiguration(Func<NancyContext, IUserIdentity> getUserIdentity)
        {
            GetUserIdentity = getUserIdentity;
        }

        /// <summary>
        /// Gets a value indicating whether the configuration is valid or not.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                if (GetUserIdentity == null)
                {
                    return false;
                }

                return true;
            }
        }
    }
}