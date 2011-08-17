using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Authentication.Basic;
using Nancy.Security;

namespace Nancy.Demo.Authentication.Basic
{
	public class UserValidator : IUserValidator
	{
		public IUserIdentity Validate(string username, string password)
		{
		    return new DemoUserIdentity {UserName = username};
		}
	}
}