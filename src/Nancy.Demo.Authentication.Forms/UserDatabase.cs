namespace Nancy.Demo.Authentication.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Authentication.Forms;

    public class UserDatabase : IUsernameMapper
    {
        private static List<Tuple<string, string, Guid>> users = new List<Tuple<string, string, Guid>>();

        static UserDatabase()
        {
            users.Add(new Tuple<string, string, Guid>("admin", "password", new Guid("55E1E49E-B7E8-4EEA-8459-7A906AC4D4C0")));
            users.Add(new Tuple<string, string, Guid>("user", "password", new Guid("56E1E49E-B7E8-4EEA-8459-7A906AC4D4C0")));
        }

        public string GetUsernameFromIdentifier(Guid identifier)
        {
            var userRecord = users.Where(u => u.Item3 == identifier).FirstOrDefault();

            if (userRecord == null)
            {
                return string.Empty;
            }

            return userRecord.Item1;
        }

        public static Guid? ValidateUser(string username, string password)
        {
            var userRecord = users.Where(u => u.Item1 == username && u.Item2 == password).FirstOrDefault();

            if (userRecord == null)
            {
                return null;
            }

            return userRecord.Item3;
        }
    }
}