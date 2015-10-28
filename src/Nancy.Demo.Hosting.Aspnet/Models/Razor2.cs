namespace Nancy.Demo.Hosting.Aspnet.Models
{
    public class Razor2
    {
        public string FirstName { get; set; }

        public string NotNullOne { get; set; }

        public string NotNullTwo { get; set; }

        public string NullOne { get; set; }

        public string NullTwo { get; set; }

        public bool FalseBool { get; set; }

        public bool TrueBool { get; set; }

        public Razor2()
        {
            this.FirstName = "Razor2";
            this.NotNullOne = "NotNullOne";
            this.NotNullTwo = "NotNullTwo";
            this.TrueBool = true;
        }
    }
}