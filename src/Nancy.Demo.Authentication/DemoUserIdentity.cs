using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Security;

namespace Nancy.Demo.Authentication
{
    public class DemoUserIdentity : IUserIdentity
    {
        public string UserName { get; set; }

        public IEnumerable<string> Claims { get; set; }
    }
}