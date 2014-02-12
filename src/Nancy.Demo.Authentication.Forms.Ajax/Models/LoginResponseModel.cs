namespace Nancy.Demo.Authentication.Forms.Ajax.Models
{  
    public class LoginResponseModel
    {
        public string Message { get; set; }
        public string AuthToken { get; set; }
        public bool IsSuccess { get; set; }
    }
}