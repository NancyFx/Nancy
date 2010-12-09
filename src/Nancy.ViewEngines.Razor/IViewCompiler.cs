using System.IO;

namespace Nancy.ViewEngines.Razor {
    public interface IViewCompiler {
        IView GetCompiledView(TextReader fullPath);
    }
}
