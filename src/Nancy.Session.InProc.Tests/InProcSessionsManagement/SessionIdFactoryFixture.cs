namespace Nancy.Session.InProc.Tests.InProcSessionsManagement
{
    using System;
    using Nancy.Session.InProc.InProcSessionsManagement;
    using Xunit;

    public class SessionIdFactoryFixture
    {
        private readonly SessionIdFactory sessionIdFactory;

        public SessionIdFactoryFixture()
        {
            this.sessionIdFactory = new SessionIdFactory();
        }

        public class CreateNew : SessionIdFactoryFixture
        {
            [Fact]
            public void Creates_non_empty_session_id()
            {
                var actual = this.sessionIdFactory.CreateNew();
                Assert.NotEqual(Guid.Empty, actual.Value);
                Assert.False(actual.IsEmpty);
            }

            [Fact]
            public void Creates_new_id_every_time()
            {
                var newId1 = this.sessionIdFactory.CreateNew();
                var newId2 = this.sessionIdFactory.CreateNew();

                Assert.NotEqual(Guid.Empty, newId1.Value);
                Assert.NotEqual(Guid.Empty, newId2.Value);
                Assert.NotEqual(newId1, newId2);
            }

            [Fact]
            public void Creates_id_marked_as_new()
            {
                var newId = this.sessionIdFactory.CreateNew();
                Assert.True(newId.IsNew);
            }
        }

        public class CreateFrom : SessionIdFactoryFixture
        {
            [Fact]
            public void Given_null_session_id_string_then_returns_null()
            {
                var actual = this.sessionIdFactory.CreateFrom(null);
                Assert.Null(actual);
            }

            [Fact]
            public void Given_empty_session_id_string_then_returns_null()
            {
                var emptySessionIdString = string.Empty;
                var actual = this.sessionIdFactory.CreateFrom(emptySessionIdString);
                Assert.Null(actual);
            }

            [Fact]
            public void Given_invalid_session_id_string_then_returns_null()
            {
                const string invalidSessionIdString = "[ThisIsNotAGuid]";
                var actual = this.sessionIdFactory.CreateFrom(invalidSessionIdString);
                Assert.Null(actual);
            }

            [Fact]
            public void Given_valid_session_id_string_then_returns_session_id()
            {
                var expectedSessionId = Guid.NewGuid();
                var sessionIdString = expectedSessionId.ToString();

                var actual = this.sessionIdFactory.CreateFrom(sessionIdString);

                Assert.NotNull(actual);
                Assert.Equal(expectedSessionId, actual.Value);
            }

            [Fact]
            public void Creates_id_marked_as_not_new()
            {
                var newId = this.sessionIdFactory.CreateFrom(Guid.NewGuid().ToString());
                Assert.False(newId.IsNew);
            }
        }
    }
}