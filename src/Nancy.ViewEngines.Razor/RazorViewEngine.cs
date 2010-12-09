
using System;
namespace Nancy.ViewEngines.Razor {
    public class RazorViewEngine {
        public RazorViewEngine()
            : this(new AspNetTemplateLocator(), new RazorViewCompiler()) {
        }

        public RazorViewEngine(IViewLocator viewTemplateLocator, IViewCompiler viewCompiler) {
            ViewTemplateLocator = viewTemplateLocator;
            ViewCompiler = viewCompiler;
        }

        public IViewLocator ViewTemplateLocator {
            get;
            private set;
        }

        public IViewCompiler ViewCompiler {
            get;
            private set;
        }

        public ViewResult RenderView(string viewTemplate, object model) {
            IView view = null;
            var result = ViewTemplateLocator.GetTemplateContents(viewTemplate);

            using (var reader = result.Contents) {
                view = ViewCompiler.GetCompiledView(reader);
            }
            if (view == null) {
                // TODO: This should be a resource string
                throw new InvalidOperationException(String.Format("Could not find a valid view at the location '{0}'", result.Location));
            }
            view.Model = model;

            return new ViewResult(view, result.Location);
        }
    }
}
