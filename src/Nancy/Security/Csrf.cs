namespace Nancy.Security
{
    using System;
    using Cookies;

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
    }
}