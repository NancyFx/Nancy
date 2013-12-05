namespace Demo.Embedded
{
    using Nancy;

    public class AccountModule : NancyModule
    {
        public AccountModule() : base("/account")
        {
            this.Get["/"] = _ => this.View["index"];
        }
    }
}