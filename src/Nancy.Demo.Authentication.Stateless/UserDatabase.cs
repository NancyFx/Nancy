namespace Nancy.Demo.Authentication.Stateless
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Security;

    public class UserDatabase
    {
        static readonly List<Tuple<string, string>> ActiveApiKeys = new List<Tuple<string, string>>();
        private static readonly List<Tuple<string, string>> Users = new List<Tuple<string, string>>();

        static UserDatabase()
        {
            Users.Add(new Tuple<string, string>("admin", "password"));
            Users.Add(new Tuple<string, string>("user", "password"));
        }

        public static IUserIdentity GetUserFromApiKey(string apiKey)
        {
            var activeKey = ActiveApiKeys.FirstOrDefault(x => x.Item2 == apiKey);

            if(activeKey==null)
            {
                return null;
            }

            var userRecord = Users.First(u => u.Item1 == activeKey.Item1);
            return new DemoUserIdentity {UserName = userRecord.Item1};
        }

        public static string ValidateUser(string username, string password)
        {
            //try to get a user from the "database" that matches the given username and password
            var userRecord = Users.FirstOrDefault(u => u.Item1 == username && u.Item2 == password);

            if(userRecord==null)
            {
                return null;
            }

            //now that the user is validated, create an api key that can be used for subsequent requests
            var apiKey = Guid.NewGuid().ToString();
            ActiveApiKeys.Add(new Tuple<string, string>(username, apiKey));
            return apiKey;
        }

        public static void RemoveApiKey(string apiKey)
        {
            var apiKeyToRemove = ActiveApiKeys.First(x => x.Item2 == apiKey);
            ActiveApiKeys.Remove(apiKeyToRemove);
        }
    }
}