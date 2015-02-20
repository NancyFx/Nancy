namespace Nancy.Tests.Unit.Security
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;

    using Nancy.Security;
    using Nancy.Tests.Fakes;

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
        public void Should_return_false_for_authentication_if_the_username_is_null()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser(null);

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_authentication_if_the_username_is_empty()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("");

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_authentication_if_the_username_is_whitespace()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("   \r\n   ");

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_authentication_if_username_is_set()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeTrue();
        }
        
        [Fact]
        public void Should_return_false_for_required_claim_if_the_user_is_null()
        {
            // Given
            ClaimsPrincipal user = null;
            var requiredClaim = new Claim("not-present-claim", "not-present-claim");

            // When
            var result = user.HasClaim(requiredClaim);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_required_claim_if_the_claims_are_null()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");
            var requiredClaim = new Claim("not-present-claim", "not-present-claim");

            // When
            var result = user.HasClaim(requiredClaim);

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_required_claim_if_the_user_does_not_have_claim()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake", new[] { new Claim("present-claim", "present-claim") });
            var requiredClaim = new Claim("not-present-claim", "not-present-claim");

            // When
            var result = user.HasClaim(requiredClaim);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_required_claim_if_the_user_does_have_claim()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake", new [] { new Claim("present-claim","present-claim") }) ;
            var requiredClaim = new Claim("present-claim", "present-claim");

            // When
            var result = user.HasClaim(requiredClaim);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_for_required_claims_if_the_user_is_null()
        {
            // Given
            ClaimsPrincipal user = null;
            var requiredClaims = new[] { new Claim("not-present-claim1", "not-present-claim1"), new Claim("not-present-claim2", "not-present-claim2") };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_required_claims_if_the_claims_are_null()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");
            var requiredClaims = new[] { new Claim("not-present-claim1", "not-present-claim1"), new Claim("not-present-claim2", "not-present-claim2") };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_required_claims_if_the_user_does_not_have_all_claims()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake", new[] { new Claim("present-claim1", "present-claim1"), new Claim("present-claim2", "present-claim2"), new Claim("present-claim3", "present-claim3") });
            var requiredClaims = new[] { new Claim("present-claim1", "present-claim1"), new Claim("not-present-claim1", "not-present-claim1") };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_required_claims_if_the_user_does_have_all_claims()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake", new[] { new Claim("present-claim1", "present-claim1"), new Claim("present-claim2", "present-claim2"), new Claim("present-claim3", "present-claim3") });
            var requiredClaims = new[] { new Claim("present-claim1", "present-claim1"), new Claim("present-claim2", "present-claim2") };

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
            var requiredClaims = new[] { new Claim("not-present-claim1", "not-present-claim1"), new Claim("not-present-claim2", "not-present-claim2") };

            // When
            var result = user.HasAnyClaim(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_any_required_claim_if_the_claims_are_null()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");
            var requiredClaims = new[] { new Claim("not-present-claim1", "not-present-claim1"), new Claim("not-present-claim2", "not-present-claim2") };

            // When
            var result = user.HasAnyClaim(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_any_required_claim_if_the_user_does_not_have_any_claim()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake", new[] { new Claim("present-claim1", "present-claim1"), new Claim("present-claim2", "present-claim2"), new Claim("present-claim3", "present-claim3") });
            var requiredClaims = new[] { new Claim("not-present-claim1", "not-present-claim1"), new Claim("not-present-claim2", "not-present-claim2") };

            // When
            var result = user.HasAnyClaim(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_any_required_claim_if_the_user_does_have_any_of_claim()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake", new[] { new Claim("present-claim1", "present-claim1"), new Claim("present-claim2", "present-claim2"), new Claim("present-claim3", "present-claim3") });
            var requiredClaims = new[] { new Claim("present-claim1","present-claim1"), new Claim("not-present-claim1","not-present-claim1") };

            // When
            var result = user.HasAnyClaim(requiredClaims);

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
        public void Should_return_false_for_valid_claim_if_claims_are_null()
        {
            // Given
            ClaimsPrincipal user = GetFakeUser("Fake");
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
            ClaimsPrincipal user = GetFakeUser("Fake", new[] { new Claim("present-claim1", "present-claim1"), new Claim("present-claim2", "present-claim2"), new Claim("present-claim3", "present-claim3") });
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
            ClaimsPrincipal user = GetFakeUser("Fake", new[] { new Claim("present-claim1","present-claim1"), new Claim("present-claim2","present-claim2"), new Claim("present-claim3","present-claim3") });
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
            IEnumerable<Claim> userClaims = new Claim[] {};
            ClaimsPrincipal user = GetFakeUser("Fake", userClaims);

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
            validatedClaims.ShouldBeSameAs(userClaims);
        }

        private static ClaimsPrincipal GetFakeUser(string userName, IEnumerable<Claim> claims = null)
        {
            var ret = new ClaimsPrincipal();
            //TODO: probably should add userName as a Name claim
            ret.AddIdentity(new ClaimsIdentity(claims, "Test"));

            return ret;
        }
    }
}