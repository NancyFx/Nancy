namespace Nancy.Demo.Authentication.Models
{
    public class UserModel
    {
        public UserModel(string username)
        {
            this.Username = username;
        }

        public string Username { get; set; }
    }
}