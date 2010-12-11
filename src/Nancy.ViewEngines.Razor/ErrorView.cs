namespace Nancy.ViewEngines.Razor
{
    public class ErrorView : RazorViewBase
    {
        public ErrorView(string message) {
            Message = message;
        }

        public string Message { get; private set; }

        public override void WriteLiteral(object value)
        {
            base.WriteLiteral(Message);
        }

        public override void Execute()
        {
            base.WriteLiteral(Message);
            base.WriteLiteral("<hr />");
            base.WriteLiteral(Code);
        }
    }
}
