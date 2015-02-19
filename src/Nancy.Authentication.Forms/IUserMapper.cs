namespace Nancy.Authentication.Forms
{
    using System;
    using System.Security.Claims;

    /// <summary>
    /// Provides a mapping between forms auth guid identifiers and
    /// real usernames
    /// </summary>
    public interface IUserMapper
    {
        /// <summary>
        /// Get the real username from an identifier
        /// </summary>
        /// <param name="identifier">User identifier</param>
        /// <param name="context">The current NancyFx context</param>
        /// <returns>Matching populated IUserIdentity object, or empty</returns>
        ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context);
    }
}