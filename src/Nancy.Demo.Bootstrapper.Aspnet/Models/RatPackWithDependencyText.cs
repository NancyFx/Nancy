namespace Nancy.Demo.Bootstrapping.Aspnet.Models
{
    public class RatPackWithDependencyText : RatPack
    {
        public string ApplicationDependencyText { get; set; }

        public string RequestDependencyText { get; set; }
    }
}