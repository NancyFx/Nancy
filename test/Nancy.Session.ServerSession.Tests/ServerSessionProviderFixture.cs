namespace Nancy.Session.ServerSession.Tests
{
    using Nancy.Testing;
    using System.Threading.Tasks;
    using Xunit;

    public class ServerSessionProviderFixture
    {
        public ServerSessionProviderFixture()
        {
        }

        [Fact]
        public async Task should_save_and_return_session_value()
        {
            Browser browser = new Browser(with => {
                with.Module<SessionModule>();
            });
            var response = await browser.Get("/getsession");
            string result = response.Body.AsString();
            Assert.Contains(SessionModule.expectedSessionValue, result);
        }

        [Fact]
        public void should_be_using_ServerSessionProvider()
        {
            Browser browser = new Browser(with => { });
            Assert.Equal(typeof(ServerSessionProvider),SessionProvider.CurrentSessionProviderType);
        }
        [Fact]
        public void should_be_using_ServerSessionDependency()
        {
            Browser browser = new Browser(with => {});
            Assert.Equal(typeof(ServerSessionDependency), SessionProvider.CurrentSessionDependencyType);
        }


        public class SessionModule : NancyModule
        {
            internal const string expectedSessionValue = "abc";
            public SessionModule()
            {
                Get<object>("getsession", p => {
                    if (Session["test"] == null)
                    {
                        Session["test"] = expectedSessionValue;
                    }

                    return Session["test"];
                });
            }
        }

    }
}