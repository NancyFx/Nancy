namespace Nancy.Authentication.Forms
{
    using Nancy.Security;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an asynchronous mapping between forms auth guid identifiers
    /// and real usernames
    /// </summary>
    public interface IAsyncUserMapper
    {
        /// <summary>
        /// Get the real username from an identifier
        /// </summary>
        /// <param name="identifier">
        /// User identifier
        /// </param>
        /// <param name="context">
        /// The current NancyFx context
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token that can be used by other objects or threads to
        /// receive notice of cancellation.
        /// </param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<IUserIdentity> GetUserFromIdentifierAsync(Guid identifier, NancyContext context, CancellationToken cancellationToken);
    }
}
