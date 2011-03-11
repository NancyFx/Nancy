namespace Nancy.Demo.Authentication.Models
{
    public class UserModel
    {
        public string Username { get; set; }

        public UserModel(string username)
        {
            this.Username = username;
        }
    }
}