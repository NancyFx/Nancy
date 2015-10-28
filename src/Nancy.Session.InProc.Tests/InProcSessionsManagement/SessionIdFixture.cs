namespace Nancy.Session.InProc.Tests.InProcSessionsManagement
{
    using System;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Xunit;

    public class SessionIdFixture
    {
        [Fact]
        public void Given_empty_value_then_isempty_returns_true()
        {
            var emptySessionId = new SessionId(Guid.Empty, false);
            Assert.True(emptySessionId.IsEmpty);
        }

        [Fact]
        public void Constructor_fills_properties()
        {
            var value = Guid.NewGuid();
            var emptySessionId = new SessionId(value, true);
            Assert.Equal(emptySessionId.Value, value);
            Assert.Equal(emptySessionId.IsNew, true);
        }

        [Fact]
        public void Objects_with_equal_values_are_equal()
        {
            var value = Guid.NewGuid();
            var sessionId1 = new SessionId(value, true);
            var sessionId2 = new SessionId(value, false);
            Assert.True(sessionId1.Equals(sessionId2));
        }

        [Fact]
        public void Can_compare_with_guids()
        {
            var value = Guid.NewGuid();
            var sessionId1 = new SessionId(value, true);
            var sessionId2 = new SessionId(Guid.NewGuid(), false);
            Assert.True(sessionId1.Equals(value));
            Assert.False(sessionId2.Equals(value));
        }

        [Fact]
        public void Supports_equality_operator()
        {
            var value = Guid.NewGuid();
            var sessionId1 = new SessionId(value, true);
            var sessionId2 = new SessionId(value, false);
            var sessionId3 = new SessionId(Guid.NewGuid(), false);
            Assert.True(sessionId1 == sessionId2);
            Assert.False(sessionId1 == sessionId3);
            Assert.False(sessionId2 == sessionId3);
        }

        [Fact]
        public void Supports_inequality_operator()
        {
            var value = Guid.NewGuid();
            var sessionId1 = new SessionId(value, true);
            var sessionId2 = new SessionId(value, false);
            var sessionId3 = new SessionId(Guid.NewGuid(), false);
            Assert.False(sessionId1 != sessionId2);
            Assert.True(sessionId1 != sessionId3);
            Assert.True(sessionId2 != sessionId3);
        }

        [Fact]
        public void Null_objects_are_equal()
        {
            SessionId one = null;
            SessionId two = null;
            Assert.True(one == two);
        }

        [Fact]
        public void A_null_session_id_is_not_equal_to_a_non_null_session_id()
        {
            SessionId one = null;
            var two = new SessionId(Guid.NewGuid(), true);
            Assert.False(one == two);
        }
    }
}