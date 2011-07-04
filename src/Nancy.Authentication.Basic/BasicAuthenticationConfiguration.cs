using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Authentication.Basic
{
	/// <summary>
	/// Configuration options for forms authentication
	/// </summary>
	public class BasicAuthenticationConfiguration
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BasicAuthenticationConfiguration"/> class.
        /// </summary>
		/// <param name="userValidator">A valid instance of <see cref="IUserValidator"/> class</param>
		/// <param name="realm">Basic authentication realm</param>
		public BasicAuthenticationConfiguration(IUserValidator userValidator, string realm)
        {
			if (userValidator == null)
				throw new ArgumentNullException("userValidator");

			if (string.IsNullOrEmpty(realm))
				throw new ArgumentException("realm");

			this.UserValidator = userValidator;
			this.Realm = realm;
        }

		/// <summary>
		/// Gets the user validator
		/// </summary>
		public IUserValidator UserValidator
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the basic authentication realm
		/// </summary>
		public string Realm
		{
			get;
			private set;
		}
	}
}
