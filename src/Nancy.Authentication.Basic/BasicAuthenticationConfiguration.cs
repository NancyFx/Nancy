namespace Nancy.Authentication.Basic
{
    using System;

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
        /// <param name="userPromptBehaviour">Control when the browser should be instructed to prompt for credentials</param>
        public BasicAuthenticationConfiguration(IUserValidator userValidator, string realm, UserPromptBehaviour userPromptBehaviour = UserPromptBehaviour.NonAjax)
        {
            if (userValidator == null)
            {
                throw new ArgumentNullException("userValidator");
            }

            if (string.IsNullOrEmpty(realm))
            {
                throw new ArgumentException("realm");
            }

            this.UserValidator = userValidator;
            this.Realm = realm;
            this.UserPromptBehaviour = userPromptBehaviour;
        }

        /// <summary>
        /// Gets the user validator
        /// </summary>
        public IUserValidator UserValidator { get; private set; }

        /// <summary>
        /// Gets the basic authentication realm
        /// </summary>
        public string Realm { get; private set; }

        /// <summary>
        /// Determines when the browser should prompt the user for credentials
        /// </summary>
        public UserPromptBehaviour UserPromptBehaviour { get; private set; }
    }
}
