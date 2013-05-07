namespace Nancy.Demo.Authentication.Forms.Models
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