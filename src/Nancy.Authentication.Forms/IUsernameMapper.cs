using Nancy.Security;

namespace Nancy.Authentication.Forms
{
    using System;

    /// <summary>
    /// Provides a mapping between forms auth guid identifiers and
    /// real usernames
    /// </summary>
    public interface IUserMapper
    {
        /// <summary>
        /// Get the real username from an indentifier
        /// </summary>
        /// <param name="indentifier">User identifier</param>
        /// <returns>Matching username, or empty</returns>
        IUserIdentity GetUserFromIdentifier(Guid indentifier);
    }
}