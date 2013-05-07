namespace Nancy.Demo.Authentication.Stateless.Models
{
    public class UserModel
    {
        public UserModel(string username)
        {
            Username = username;
        }

        public string Username { get; private set; }
    }
}