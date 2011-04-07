namespace Nancy.Demo.ModelBinding
{
    using System.Text;
    using Models;
    using Nancy.ModelBinding;

    public class XmlModule : NancyModule
    {
        public XmlModule()
        {
            Get["/bindxml"] = x =>
            {
                return View["PostXml"];
            };

            Post["/bindxml"] = x =>
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