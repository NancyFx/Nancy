namespace Nancy.ViewEngines.Razor
{
    using System.IO;

    public interface IView
    {
        string Code { get; set; }

        object Model { get; set; }

        TextWriter Writer { get; set; }

        void Execute();
    }
}
