using System;
using System.IO;

namespace Nancy.ViewEngines.Razor {
    public static class RazorViewEngineExtensions {
        public static Action<Stream> Razor(this IViewEngine source, string name) {
            return source.Razor(name, (object)null);
        }

        public static Action<Stream> Razor<TModel>(this IViewEngine source, string name, TModel model) {
            var viewEngine = new RazorViewEngine();

            return stream => {
                var result = viewEngine.RenderView(name, model);
                result.Execute(stream);
            };
        }
    }
}
