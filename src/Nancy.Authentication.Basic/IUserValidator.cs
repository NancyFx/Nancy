using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Authentication.Basic
{
	/// <summary>
	/// Provides a way to validate the username and password
	/// </summary>
	public interface IUserValidator
	{
		/// <summary>
		/// Validates the username and password
		/// </summary>
		/// <param name="username">Username</param>
		/// <param name="password">Password</param>
		/// <returns>A value indicating whether the user was authenticated or not</returns>
		bool Validate(string username, string password);
	}
}
