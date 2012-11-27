namespace Nancy.Tests.Unit.Sessions
{
    using System.Collections.Generic;
    using FakeItEasy;
    using Nancy.Session;
    using Xunit;

    public class SessionHooksFixture
    {
        private readonly NancyContext ctx;

        public SessionHooksFixture()
        {
            ctx = new NancyContext();
            ctx.Request = new Request("Get", "/", "http");
            ctx.SessionStore = A.Fake<ISessionStore>();
        }

        [Fact]
        public void Should_create_an_empty_session_when_none_is_present()
        {
            IDictionary<string, object> items;
            A.CallTo(() => ctx.SessionStore.TryLoadSession(A<Request>.Ignored, out items)).Returns(false);

            SessionApplicationStartup.LoadSession(ctx);

            Assert.NotNull(ctx.Request.Session);
        }

        [Fact]
        public void Should_load_a_session_if_it_exists()
        {
            IDictionary<string, object> items;
            A.CallTo(() => ctx.SessionStore.TryLoadSession(A<Request>.Ignored, out items))
                .Returns(true)
                .AssignsOutAndRefParameters(new Dictionary<string, object> { { "Hello", "World" } });

            SessionApplicationStartup.LoadSession(ctx);

            Assert.NotNull(ctx.Request.Session);
            Assert.Equal("World", ctx.Request.Session.Hello);
        }

        [Fact]
        public void Should_not_save_an_unchanged_session()
        {
            ctx.Request.Session = new DynamicSession();

            SessionApplicationStartup.SaveSession(ctx);
            A.CallTo(() => ctx.SessionStore.SaveSession(A<NancyContext>.Ignored, A<IDictionary<string, object>>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public void Should_save_a_changed_session()
        {
            ctx.Request.Session = new DynamicSession();
            ctx.Request.Session.Hello = "World";

            SessionApplicationStartup.SaveSession(ctx);
            A.CallTo(() => ctx.SessionStore.SaveSession(A<NancyContext>.Ignored, A<IDictionary<string, object>>.Ignored))
                .WhenArgumentsMatch(args =>
                {
                    var items = args[1] as IDictionary<string, dynamic>;
                    return items != null && items["Hello"] == "World";
                })
                .MustHaveHappened();
        }
    }
}
