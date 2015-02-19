
namespace Nancy.Testing
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Nancy.Authentication.Forms;
    using Nancy.Extensions;
    using Nancy.Helpers;
    using Nancy.Responses;
    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Defines extensions for the <see cref="BrowserContext"/> type.
    /// </summary>
    public static class BrowserContextExtensions
    {
        /// <summary>
        /// Adds a multipart/form-data encoded request body to the <see cref="Browser"/>, using the default boundary name.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="multipartFormData">The multipart/form-data encoded data that should be added.</param>
        public static void MultiPartFormData(this BrowserContext browserContext, BrowserContextMultipartFormData multipartFormData)
        {
            MultiPartFormData(browserContext, multipartFormData, BrowserContextMultipartFormData.DefaultBoundaryName);
        }

        /// <summary>
        /// Adds a multipart/form-data encoded request body to the <see cref="Browser"/>.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="multipartFormData">The multipart/form-data encoded data that should be added.</param>
        /// <param name="boundaryName">The name of the boundary to be used</param>
        public static void MultiPartFormData(this BrowserContext browserContext, BrowserContextMultipartFormData multipartFormData, string boundaryName)
        {
            var contextValues =
                (IBrowserContextValues)browserContext;

            contextValues.Body = multipartFormData.Body;
            contextValues.Headers["Content-Type"] = new[] { "multipart/form-data; boundary=" + boundaryName };
        }

        /// <summary>
        /// Adds a application/json request body to the <see cref="Browser"/>.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="model">The model to be serialized to json.</param>
        /// <param name="serializer">Optionally opt in to using a different JSON serializer.</param>
        public static void JsonBody<TModel>(this BrowserContext browserContext, TModel model, ISerializer serializer = null)
        {
            if (serializer == null)
            {
                serializer = new DefaultJsonSerializer();
            }

            var contextValues =
                (IBrowserContextValues)browserContext;

            contextValues.Body = new MemoryStream();

            serializer.Serialize("application/json", model, contextValues.Body);
            browserContext.Header("Content-Type", "application/json");
        }

        /// <summary>
        /// Adds a application/xml request body to the <see cref="Browser"/>.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="model">The model to be serialized to xml.</param>
        /// <param name="serializer">Optionally opt in to using a different XML serializer.</param>
        public static void XMLBody<TModel>(this BrowserContext browserContext, TModel model, ISerializer serializer = null)
        {
            if (serializer == null)
            {
                serializer = new DefaultXmlSerializer();
            }

            var contextValues =
                (IBrowserContextValues)browserContext;

            contextValues.Body = new MemoryStream();

            serializer.Serialize("application/xml", model, contextValues.Body);
            browserContext.Header("Content-Type", "application/xml");
        }

        /// <summary>
        /// Adds basic authorization credentials to the headers of the <see cref="Browser"/>.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="username">The username to be encoded.</param>
        /// <param name="password">The password to be encoded.</param>
        public static void BasicAuth(this BrowserContext browserContext, string username, string password)
        {
            var credentials = string.Format("{0}:{1}", username, password);

            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

            browserContext.Header("Authorization", "Basic " + encodedCredentials);
        }

        /// <summary>
        /// Adds a cookie to the headers of the <see cref="Browser"/>.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="cookies">The collection of cookies to add to the cookie request header.</param>
        public static void Cookie(this BrowserContext browserContext, IDictionary<string, string> cookies)
        {
            if (!cookies.Any())
            {
                return;
            }

            foreach (var cookie in cookies)
            {
                browserContext.Cookie(cookie.Key, cookie.Value);
            }
        }

        /// <summary>
        /// Adds a cookie to the headers of the <see cref="Browser"/>.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="key">The name of the cookie.</param>
        /// <param name="value">The value of the cookie.</param>
        public static void Cookie(this BrowserContext browserContext, string key, string value)
        {
            var contextValues =
                (IBrowserContextValues)browserContext;

            if (!contextValues.Headers.ContainsKey("Cookie"))
            {
                contextValues.Headers.Add("Cookie", new List<string> { string.Empty });
            }

            var values = (List<string>)contextValues.Headers["Cookie"];
            values[0] += string.Format("{0}={1};", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
        }

        /// <summary>
        /// Adds a header to indicate this request is an "ajax request"
        /// <seealso cref="RequestExtensions.IsAjaxRequest"/>
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        public static void AjaxRequest(this BrowserContext browserContext)
        {
            browserContext.Header("X-Requested-With", "XMLHttpRequest");
        }

        /// <summary>
        /// Adds forms authentication cookie to the headers of the <see cref="Browser"/>.
        /// </summary>
        /// <param name="browserContext">The <see cref="BrowserContext"/> that the data should be added to.</param>
        /// <param name="userId">The user identifier</param>
        /// <param name="formsAuthenticationConfiguration">Current configuration.</param>
        public static void FormsAuth(this BrowserContext browserContext, Guid userId, FormsAuthenticationConfiguration formsAuthenticationConfiguration)
        {
            var encryptedId = formsAuthenticationConfiguration.CryptographyConfiguration.EncryptionProvider.Encrypt(userId.ToString());

            var hmacBytes = formsAuthenticationConfiguration.CryptographyConfiguration.HmacProvider.GenerateHmac(encryptedId);

            var hmacString = Convert.ToBase64String(hmacBytes);

            var cookieContents = String.Format("{1}{0}", encryptedId, hmacString);

            Cookie(browserContext, FormsAuthentication.FormsAuthenticationCookieName, cookieContents);
        }

        public static void Accept(this BrowserContext browserContext, MediaRange mediaRange)
        {
            browserContext.Accept(mediaRange, 1.0m);
        }

        public static void Accept(this BrowserContext browserContext, MediaRange mediaRange, decimal quality)
        {
            var contextValues =
                (IBrowserContextValues)browserContext;

            if (contextValues.Headers.ContainsKey("accept"))
            {
                if (contextValues.Headers["accept"].Count().Equals(1))
                {
                    if (contextValues.Headers["accept"].Any(x => x.Equals("*/*")))
                    {
                        contextValues.Headers.Remove("accept");
                    }    
                }
            }

            var mediaTypeWithQuality =
                string.Concat(mediaRange, ";q=", Convert.ToString(quality, CultureInfo.InvariantCulture));

            browserContext.Header("accept", mediaTypeWithQuality);
        }
    }
}
