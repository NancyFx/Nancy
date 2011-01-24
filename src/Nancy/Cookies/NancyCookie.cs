namespace Nancy.Cookies
{
    using System;
    using System.Text;

    public class NancyCookie : INancyCookie
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public string Domain { get; set; }
        public string Path { get; set; }        
        public DateTime? Expires { get; set; }

        public NancyCookie(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            var sb = new StringBuilder(50);
            sb.AppendFormat("{0}={1}", Name, Value);
            if (Expires != null)
            {
                sb.Append("; expires=");
                sb.Append(Expires.Value.ToUniversalTime().ToString("ddd, dd-MMM-yyyy HH:mm:ss"));
                sb.Append(" GMT");
            }
            if (Domain != null)
            {
                sb.Append("; domain=");
                sb.Append(Domain);
            }
            if (Path != null)
            {
                sb.Append("; path=");
                sb.Append(Path);
            }
            return sb.ToString();
        }
    }
}