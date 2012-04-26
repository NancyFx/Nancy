namespace Nancy.ViewEngines.DotLiquid
{
    using System;
    using System.Text.RegularExpressions;
    using global::DotLiquid;
    using global::DotLiquid.FileSystems;
    using liquid = global::DotLiquid;    
    using System.Linq;

    /// <summary>
    /// <see cref="IFileSystem"/> implementation around the Nancy templates.
    /// </summary>
    public class LiquidNancyFileSystem : IFileSystem
    {
        private readonly ViewEngineStartupContext viewEngineStartupContext;
        private readonly IViewEngine dotLiquidViewEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiquidNancyFileSystem"/> class,
        /// with the provided <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context that the engine can operate in.</param>
        /// <param name="viewEngine">The containing view engine.</param>
        public LiquidNancyFileSystem(ViewEngineStartupContext context, DotLiquidViewEngine viewEngine)
        {
            viewEngineStartupContext = context;
            dotLiquidViewEngine = viewEngine;
        }

        /// <summary>
        /// Reads the content of the template specified by the <paramref name="templateName"/> parameter.
        /// </summary>
        /// <param name="context">The <see cref="Context"/> of the call.</param>
        /// <param name="templateName">The name of the template to read.</param>
        /// <exception cref="liquid.Exceptions.FileSystemException">The specified template could not be located.</exception>
        /// <returns>The content of the template.</returns>
        public string ReadTemplateFile(Context context, string templateName)
        {
            var neutralTemplateName = GetNeutralTemplateName(templateName);

            var viewLocation = viewEngineStartupContext.ViewLocationResults
                .FirstOrDefault(v => GetLocationQualifiedName(v).Equals(neutralTemplateName, StringComparison.OrdinalIgnoreCase));

            if (viewLocation != null)
            {
                return viewLocation.Contents.Invoke().ReadToEnd();
            }

            throw new liquid.Exceptions.FileSystemException("Template file {0} not found", new[] { templateName });
        }

        private string GetLocationQualifiedName(ViewLocationResult result)
        {
            return string.IsNullOrEmpty(result.Location) ?
                GetNeutralTemplateName(result.Name) :
                string.Concat(result.Location, "/", GetNeutralTemplateName(result.Name));
        }

        private string GetNeutralTemplateName(string templateName)
        {
            templateName = templateName
                .Replace(@"""", "")
                .Replace("'", "")
                .Replace(@"\", "/");

            // Remove all view engine extensions, but only if they appear at the end
            foreach(string extension in dotLiquidViewEngine.Extensions)
            {
                if(templateName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                {
                    templateName = templateName.Substring(0, templateName.Length - extension.Length - 1);
                    break;  // Only need to remove one extension, the rest is legitimate
                }
            }

            return templateName;
        }
    }
}