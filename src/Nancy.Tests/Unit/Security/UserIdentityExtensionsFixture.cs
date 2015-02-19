namespace Nancy.Tests.Unit.Security
{
    using System;
    using System.Collections.Generic;

    using Nancy.Security;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class UserIdentityExtensionsFixture
    {
        [Fact]
        public void Should_return_false_for_authentication_if_the_user_is_null()
        {
            // Given
            IUserIdentity user = null;

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_authentication_if_the_username_is_null()
        {
            // Given
            IUserIdentity user = GetFakeUser(null);

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_authentication_if_the_username_is_empty()
        {
            // Given
            IUserIdentity user = GetFakeUser("");

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_authentication_if_the_username_is_whitespace()
        {
            // Given
            IUserIdentity user = GetFakeUser("   \r\n   ");

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_authentication_if_username_is_set()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake");

            // When
            var result = user.IsAuthenticated();

            // Then
            result.ShouldBeTrue();
        }
        
        [Fact]
        public void Should_return_false_for_required_claim_if_the_user_is_null()
        {
            // Given
            IUserIdentity user = null;
            var requiredClaim = "not-present-claim";

            // When
            var result = user.HasClaim(requiredClaim);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_required_claim_if_the_claims_are_null()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake");
            var requiredClaim = "not-present-claim";

            // When
            var result = user.HasClaim(requiredClaim);

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_required_claim_if_the_user_does_not_have_claim()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake", new [] { "present-claim" }) ;
            var requiredClaim = "not-present-claim";

            // When
            var result = user.HasClaim(requiredClaim);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_required_claim_if_the_user_does_have_claim()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake", new [] { "present-claim" }) ;
            var requiredClaim = "present-claim";

            // When
            var result = user.HasClaim(requiredClaim);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_for_required_claims_if_the_user_is_null()
        {
            // Given
            IUserIdentity user = null;
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_required_claims_if_the_claims_are_null()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake");
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_required_claims_if_the_user_does_not_have_all_claims()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake", new[] { "present-claim1", "present-claim2", "present-claim3" });
            var requiredClaims = new[] { "present-claim1", "not-present-claim1" };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_required_claims_if_the_user_does_have_all_claims()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake", new[] { "present-claim1", "present-claim2", "present-claim3" });
            var requiredClaims = new[] { "present-claim1", "present-claim2" };

            // When
            var result = user.HasClaims(requiredClaims);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_for_any_required_claim_if_the_user_is_null()
        {
            // Given
            IUserIdentity user = null;
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasAnyClaim(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_any_required_claim_if_the_claims_are_null()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake");
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasAnyClaim(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }
        
        [Fact]
        public void Should_return_false_for_any_required_claim_if_the_user_does_not_have_any_claim()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake", new[] { "present-claim1", "present-claim2", "present-claim3" });
            var requiredClaims = new[] { "not-present-claim1", "not-present-claim2" };

            // When
            var result = user.HasAnyClaim(requiredClaims);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_any_required_claim_if_the_user_does_have_any_of_claim()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake", new[] { "present-claim1", "present-claim2", "present-claim3" });
            var requiredClaims = new[] { "present-claim1", "not-present-claim1" };

            // When
            var result = user.HasAnyClaim(requiredClaims);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_false_for_valid_claim_if_the_user_is_null()
        {
            // Given
            IUserIdentity user = null;
            Func<IEnumerable<string>, bool> isValid = claims => true;

            // When
            var result = user.HasValidClaims(isValid);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_valid_claim_if_claims_are_null()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake");
            Func<IEnumerable<string>, bool> isValid = claims => true;

            // When
            var result = user.HasValidClaims(isValid);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_false_for_valid_claim_if_the_validation_fails()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake", new[] { "present-claim1", "present-claim2", "present-claim3" });
            Func<IEnumerable<string>, bool> isValid = claims => false;

            // When
            var result = user.HasValidClaims(isValid);

            // Then
            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_return_true_for_valid_claim_if_the_validation_succeeds()
        {
            // Given
            IUserIdentity user = GetFakeUser("Fake", new[] { "present-claim1", "present-claim2", "present-claim3" });
            Func<IEnumerable<string>, bool> isValid = claims => true;

            // When
            var result = user.HasValidClaims(isValid);

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_call_validation_with_users_claims()
        {
            // Given
            IEnumerable<string> userClaims = new string[0];
            IUserIdentity user = GetFakeUser("Fake", userClaims);

            IEnumerable<string> validatedClaims = null;
            Func<IEnumerable<string>, bool> isValid = claims =>
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

        private static IUserIdentity GetFakeUser(string userName, IEnumerable<string> claims = null)
        {
            var ret = new FakeUserIdentity();
            ret.UserName = userName;
            ret.Claims = claims;
            
            return ret;
        }
    }
}