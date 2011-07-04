using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.Authentication.Basic;

namespace Nancy.Demo.Authentication.Basic
{
	public class UserValidator : IUserValidator
	{
		public bool Validate(string username, string password)
		{
			return username == "foo" && password == "bar";
		}
	}
}