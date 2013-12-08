namespace Nancy.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension methods for working with IUserIdentity.
    /// </summary>
    public static class UserIdentityExtensions
    {
        /// <summary>
        /// Tests if the user is authenticated.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <returns>True if the user is authenticated, false otherwise</returns>
        public static bool IsAuthenticated(this IUserIdentity user)
        {
            return
                user != null
                && !String.IsNullOrWhiteSpace(user.UserName);
        }

        /// <summary>
        /// Tests if the user has a required claim.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <param name="requiredClaim">Claim the user needs to have</param>
        /// <returns>True if the user has the required claim, false otherwise</returns>
        public static bool HasClaim(this IUserIdentity user, string requiredClaim)
        {
            return
                user != null
                && user.Claims != null
                && user.Claims.Contains(requiredClaim, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Tests if the user has all of the required claims.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <param name="requiredClaims">Claims the user needs to have</param>
        /// <returns>True if the user has all of the required claims, false otherwise</returns>
        public static bool HasClaims(this IUserIdentity user, IEnumerable<string> requiredClaims)
        {
            return
                user != null
                && user.Claims != null
                && !requiredClaims.Any(claim => !user.Claims.Contains(claim, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tests if the user has at least one of the required claims.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <param name="requiredClaims">Claims the user needs to have at least one of</param>
        /// <returns>True if the user has at least one of the required claims, false otherwise</returns>
        public static bool HasAnyClaim(this IUserIdentity user, IEnumerable<string> requiredClaims)
        {
            return
                user != null
                && user.Claims != null
                && requiredClaims.Any(claim => user.Claims.Contains(claim, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tests if the user has claims that satisfy the supplied validation function.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <param name="isValid">Validation function to be called with the authenticated
        /// users claims</param>
        /// <returns>True if the user does pass the supplied validation function, false otherwise</returns>
        public static bool HasValidClaims(this IUserIdentity user, Func<IEnumerable<string>, bool> isValid)
        {
            return
                user != null
                && user.Claims != null
                && isValid(user.Claims);
        }
    }
}