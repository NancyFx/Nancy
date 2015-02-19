namespace Nancy.Authentication.Stateless
{
    using System;

    using Nancy.Security;

    /// <summary>
    /// Configuration options for stateless authentication
    /// </summary>
    public class StatelessAuthenticationConfiguration
    {
        internal Func<NancyContext, IUserIdentity> GetUserIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatelessAuthenticationConfiguration"/> class.
        /// </summary>
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
                return this.GetUserIdentity != null;
            }
        }
    }
}