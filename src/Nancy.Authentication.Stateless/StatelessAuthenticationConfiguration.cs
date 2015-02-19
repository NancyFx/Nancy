namespace Nancy.Authentication.Stateless
{
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Configuration options for stateless authentication
    /// </summary>
    public class StatelessAuthenticationConfiguration
    {
        internal readonly Func<NancyContext, ClaimsPrincipal> GetUserIdentity;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatelessAuthenticationConfiguration"/> class.
        /// </summary>
        public StatelessAuthenticationConfiguration(Func<NancyContext, ClaimsPrincipal> getUserIdentity)
        {
            this.GetUserIdentity = getUserIdentity;
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