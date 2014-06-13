namespace Nancy.Authentication.Token
{
    using System.Collections.Generic;

    using Nancy.Security;

    public interface IUserMapper
    {
        IUserIdentity GetUser(string userName, IEnumerable<string> claims);
    }
}