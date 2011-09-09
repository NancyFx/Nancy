namespace Nancy.Security
{
    using System;
    using Cookies;

    using Nancy.Helpers;

    public static class Csrf
    {
         public static Response WithCsrfToken(this Response response, NancyContext context, string salt = null)
         {
             var token = new CsrfToken
                             {
                                 CreatedDate = DateTime.Now,
                                 Salt = salt ?? string.Empty
                             };
             token.CreateRandomBytes();
             token.CreateHmac(CsrfStartup.CryptographyConfiguration.HmacProvider);

             var tokenString = CsrfStartup.ObjectSerializer.Serialize(token);

             response.AddCookie(new NancyCookie(CsrfToken.DEFAULT_CSRF_KEY, tokenString, true));
             context.Items[CsrfToken.DEFAULT_CSRF_KEY] = tokenString;

             return response;
         }

        public static void ValidateCsrfToken(this NancyModule module, TimeSpan? validityPeriod = null)
        {
            var request = module.Request;

            if (request == null)
            {
                return;
            }

            CsrfToken cookieToken = null;
            CsrfToken formToken = null;

            string cookieTokenString;
            if (request.Cookies.TryGetValue(CsrfToken.DEFAULT_CSRF_KEY, out cookieTokenString))
            {
                cookieToken = (CsrfToken)CsrfStartup.ObjectSerializer.Deserialize(HttpUtility.UrlDecode(cookieTokenString));
            }

            var formTokenString = request.Form[CsrfToken.DEFAULT_CSRF_KEY].Value;
            if (formTokenString != null)
            {
                formToken = (CsrfToken)CsrfStartup.ObjectSerializer.Deserialize(formTokenString);
            }

            var result = CsrfStartup.TokenValidator.Validate(cookieToken, formToken, null, validityPeriod);

            if (result != CsrfTokenValidationResult.Ok)
            {
                throw new CsrfValidationException(result);
            }
        }
    }
}