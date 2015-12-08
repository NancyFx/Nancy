namespace Nancy.Demo.ModelBinding
{
    using System.Text;

    using Nancy.Demo.ModelBinding.Models;
    using Nancy.ModelBinding;

    public class JsonModule : NancyModule
    {
        public JsonModule()
        {
            Get["/bindjson"] = x =>
                {
                    return View["PostJson"];
                };

            Post["/bindjson"] = x =>
                {
                    User model = this.Bind();

                    var sb = new StringBuilder();

                    sb.AppendLine("Bound Model:");
                    sb.Append("Type: ");
                    sb.AppendLine(model.GetType().FullName);
                    sb.Append("Name: ");
                    sb.AppendLine(model.Name);
                    sb.Append("Address: ");
                    sb.AppendLine(model.Address);

                    return sb.ToString();
                };
        }
    }
}