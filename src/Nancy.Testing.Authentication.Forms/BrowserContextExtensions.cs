namespace Nancy.Testing.Authentication.Forms
{
    using System;
    using Nancy.Authentication.Forms;

    /// <summary>
    /// Defines extensions for the <see cref="BrowserContext"/> type for using Forms Authentication from tests.
    /// </summary>
    public static class BrowserContextExtensions
    {
        /// <summary>
        /// Adds forms authentication cookie to the headers of the <see cref="Browser"/>.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="formsAuthenticationConfiguration">Current configuration.</param>
        public static void FormsAuth(this BrowserContext browserContext, Guid userId, FormsAuthenticationConfiguration formsAuthenticationConfiguration)
        {
            var encryptedId = 
                formsAuthenticationConfiguration.CryptographyConfiguration.EncryptionProvider.Encrypt(userId.ToString());

            var hmacBytes = 
                formsAuthenticationConfiguration.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedId);

            var hmacString = 
                Convert.ToBase64String(hmacBytes);

            var cookieContents = 
                String.Format("{1}{0}", encryptedId, hmacString);

            browserContext.Cookie(FormsAuthentication.FormsAuthenticationCookieName, cookieContents);
        }
    }
}
