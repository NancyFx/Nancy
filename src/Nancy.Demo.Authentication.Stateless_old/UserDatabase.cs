namespace Nancy.Demo.Authentication.Stateless
{
    using Nancy.Security;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class UserDatabase
    {
        private static List<Tuple<string, string, string>> users = new List<Tuple<string, string, string>>();

        static UserDatabase()
        {
            users.Add(new Tuple<string, string, string>("admin", "password", "55E1E49E-B7E8-4EEA-8459-7A906AC4D4C0"));
            users.Add(new Tuple<string, string, string>("user", "password", "56E1E49E-B7E8-4EEA-8459-7A906AC4D4C0"));
        }

        public static IUserIdentity GetUserFromApiKey(string apiKey)
        {
            var userRecord = users.FirstOrDefault(u => u.Item3 == apiKey);

            return userRecord == null
                       ? null
                       : new DemoUserIdentity {UserName = userRecord.Item1};
        }

        public static string ValidateUser(string username, string password)
        {
            var userRecord = users.FirstOrDefault(u => u.Item1 == username && u.Item2 == password);

            return userRecord == null
                       ? null
                       : userRecord.Item3;
        }
    }
}