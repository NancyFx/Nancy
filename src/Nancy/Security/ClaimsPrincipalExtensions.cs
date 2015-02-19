namespace Nancy.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    /// Extension methods for working with IUserIdentity.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Tests if the user is authenticated.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <returns>True if the user is authenticated, false otherwise</returns>
        public static bool IsAuthenticated(this ClaimsPrincipal user)
        {
            return user != null && user.Identity != null && user.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Tests if the user has a required claim.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <param name="match">Claim the user needs to have</param>
        /// <returns>True if the user has the required claim, false otherwise</returns>
        public static bool HasClaim(this ClaimsPrincipal user, Predicate<Claim> match)
        {
            return user != null && user.FindFirst(match) != null;
        }

        /// <summary>
        /// Tests if the user has all of the required claims.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <param name="requiredClaims">Claims the user needs to have</param>
        /// <returns>True if the user has all of the required claims, false otherwise</returns>
        public static bool HasClaims(this ClaimsPrincipal user, IEnumerable<Predicate<Claim>> requiredClaims)
        {
            return user != null && requiredClaims.All(match => user.FindFirst(match) != null);
        }

        /// <summary>
        /// Tests if the user has at least one of the required claims.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <param name="requiredClaims">Claims the user needs to have at least one of</param>
        /// <returns>True if the user has at least one of the required claims, false otherwise</returns>
        public static bool HasAnyClaim(this ClaimsPrincipal user, IEnumerable<string> requiredClaims)
        {
            return user != null && requiredClaims.Any(match => user.FindFirst(match) != null);
        }

        /// <summary>
        /// Tests if the user has claims that satisfy the supplied validation function.
        /// </summary>
        /// <param name="user">User to be verified</param>
        /// <param name="isValid">Validation function to be called with the authenticated
        /// users claims</param>
        /// <returns>True if the user does pass the supplied validation function, false otherwise</returns>
        public static bool HasValidClaims(this ClaimsPrincipal user, Func<IEnumerable<Claim>, bool> isValid)
        {
            return
                user != null
                && user.Claims != null
                && isValid(user.Claims);
        }
    }
}