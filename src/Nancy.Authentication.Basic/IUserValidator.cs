namespace Nancy.Authentication.Basic
{
    using Nancy.Security;

    /// <summary>
    /// Provides a way to validate the username and password
    /// </summary>
    public interface IUserValidator
    {
        /// <summary>
        /// Validates the username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>A value representing the authenticated user, null if the user was not authenticated.</returns>
        IUserIdentity Validate(string username, string password);
    }
}
