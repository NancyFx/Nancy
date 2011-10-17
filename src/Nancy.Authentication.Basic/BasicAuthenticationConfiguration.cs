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
        /// <param name="promptUser">Tell the browser to prompt the user for credentials</param>
		public BasicAuthenticationConfiguration(IUserValidator userValidator, string realm, bool promptUser = true)
        {
			if (userValidator == null)
				throw new ArgumentNullException("userValidator");

			if (string.IsNullOrEmpty(realm))
				throw new ArgumentException("realm");

			this.UserValidator = userValidator;
			this.Realm = realm;
            this.PromptUser = promptUser;
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

        /// <summary>
        /// Determines whether the browser should prompt for credentials
        /// </summary>
        public bool PromptUser
        {
            get;
            private set;
        }
	}
}
