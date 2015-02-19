namespace Nancy.Tests.Unit.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;

    using Nancy.Security;

    using Xunit;

    public class ClaimsPrincipalExtensionsFixture
    {
        [Fact]
        public void Should_return_false_for_authentication_if_the_user_is_null()
        {
            // Given
            ClaimsPrincipal user = null;

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_authentication_if_the_identity_is_anonymous()
        {
            // Given
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity());

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_required_claim_if_the_claims_are_null()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");
            var requiredClaim = "not-present-claim";

            // When
            var result = user.HasClaim(c => c.Type == requiredClaim);

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_required_claim_if_the_user_does_not_have_claim()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake", new Claim("present-claim", string.Empty));
            var requiredClaim = "not-present-claim";

            // When
            var result = user.HasClaim(c => c.Type == requiredClaim);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_required_claim_if_the_user_does_have_claim()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake", new Claim("present-claim", string.Empty));
            var requiredClaim = "present-claim";

            // When
            var result = user.HasClaim(c => c.Type == requiredClaim);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_for_required_claims_if_the_user_is_null()
        {
            // Given
            ClaimsPrincipal user = null;
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasClaims(c => requiredClaims.Contains(c.Type));

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_required_claims_if_the_claims_are_null()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasClaims(c => requiredClaims.Contains(c.Type));

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_required_claims_if_the_user_does_not_have_all_claims()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake",
                new Claim("present-claim1", string.Empty),
                new Claim("present-claim2", string.Empty),
                new Claim("present-claim3", string.Empty));
            var requiredClaims = new Predicate<Claim>[]
            {
                c => c.Type == "present-claim1",
                c => c.Type == "not-present-claim1"
            };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_required_claims_if_the_user_does_have_all_claims()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake",
                new Claim("present-claim1", string.Empty),
                new Claim("present-claim2", string.Empty),
                new Claim("present-claim3", string.Empty));
            var requiredClaims = new Predicate<Claim>[]
            {
                c => c.Type == "present-claim1",
                c => c.Type == "present-claim2"
            };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_for_any_required_claim_if_the_user_is_null()
        {
            // Given
            ClaimsPrincipal user = null;
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasAnyClaim(c => requiredClaims.Contains(c.Type));

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_any_required_claim_if_the_claims_are_null()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasAnyClaim(c => requiredClaims.Contains(c.Type));

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_any_required_claim_if_the_user_does_not_have_any_claim()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake",
                new Claim("present-claim1", string.Empty),
                new Claim("present-claim2", string.Empty),
                new Claim("present-claim3", string.Empty));
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasAnyClaim(c => requiredClaims.Contains(c.Type));

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_any_required_claim_if_the_user_does_have_any_of_claim()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake",
                new Claim("present-claim1", string.Empty),
                new Claim("present-claim2", string.Empty),
                new Claim("present-claim3", string.Empty));
            var requiredClaims = new[] { "present-claim1", "not-present-claim1" };

            // When
            var result = user.HasAnyClaim(c => requiredClaims.Contains(c.Type));

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_for_valid_claim_if_the_user_is_null()
        {
            // Given
            ClaimsPrincipal user = null;
            Func<IEnumerable<Claim>, bool> isValid = claims => true;

            // When
            var result = user.HasValidClaims(isValid);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_valid_claim_if_the_validation_fails()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake",
                new Claim("present-claim1", string.Empty),
                new Claim("present-claim2", string.Empty),
                new Claim("present-claim3", string.Empty));
            Func<IEnumerable<Claim>, bool> isValid = claims => false;

            // When
            var result = user.HasValidClaims(isValid);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_valid_claim_if_the_validation_succeeds()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake",
                new Claim("present-claim1", string.Empty),
                new Claim("present-claim2", string.Empty),
                new Claim("present-claim3", string.Empty));
            Func<IEnumerable<Claim>, bool> isValid = claims => true;

            // When
            var result = user.HasValidClaims(isValid);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_call_validation_with_users_claims()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");

            IEnumerable<Claim> validatedClaims = null;
            Func<IEnumerable<Claim>, bool> isValid = claims =>
            {
                // store passed claims for testing assertion
                validatedClaims = claims;
                return true;
            };

            // When
            user.HasValidClaims(isValid);

            // Then
            validatedClaims.ShouldEqualSequence(user.Claims);
        }

        private static ClaimsPrincipal GetFakeUser(string userName, params Claim[] claims)
        {
            var claimsList = (claims ?? Enumerable.Empty<Claim>()).ToList();
            claimsList.Add(new Claim(ClaimTypes.NameIdentifier, userName));

            return new ClaimsPrincipal(new ClaimsIdentity(claimsList));
        }
    }
}