namespace Nancy.Demo.ModelBinding
{
    using System.Text;

    using Nancy.Demo.ModelBinding.Models;
    using Nancy.ModelBinding;

    public class XmlModule : NancyModule
    {
        public XmlModule()
        {
            Get("/bindxml", args =>
            {
                return View["PostXml"];
            });

            Post("/bindxml", args =>
            {
                var model = this.Bind<User>(u => u.Name);

                var sb = new StringBuilder();

                sb.AppendLine("Bound Model:");
                sb.Append("Type: ");
                sb.AppendLine(model.GetType().FullName);
                sb.Append("Name: (which will be empty because it's ignored)");
                sb.AppendLine(model.Name);
                sb.Append("Address: ");
                sb.AppendLine(model.Address);

                return sb.ToString();
            });
        }
    }
}