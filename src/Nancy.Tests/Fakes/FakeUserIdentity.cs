namespace Nancy.Tests.Fakes
{
    using System.Collections.Generic;

    using Nancy.Security;

    public class FakeUserIdentity : IUserIdentity
    {
        public string UserName { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }
}