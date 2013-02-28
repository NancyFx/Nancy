namespace Nancy.Authentication.Forms.Tests.Fakes
{
    using System.Collections.Generic;
    using Security;

    public class FakeUserIdentity : IUserIdentity
    {
        public string UserName { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }
}