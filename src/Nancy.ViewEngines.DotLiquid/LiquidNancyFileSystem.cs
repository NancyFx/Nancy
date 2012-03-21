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
        private readonly Regex extensionExpression;
        private readonly ViewEngineStartupContext nancyContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiquidNancyFileSystem"/> class,
        /// with the provided <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context that the engine can operate in.</param>
        public LiquidNancyFileSystem(ViewEngineStartupContext context)
        {
            this.nancyContext = context;
            this.extensionExpression = new Regex(".liquid", RegexOptions.IgnoreCase | RegexOptions.IgnoreCase);
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
            var neutralTemplateName =
                this.GetNeutralTemplateName(templateName);

            var viewLocation = nancyContext.ViewLocationResults
                .FirstOrDefault(v => GetLocationQualifiedName(v).Equals(neutralTemplateName, StringComparison.OrdinalIgnoreCase));

            if (viewLocation != null)
            {
                return viewLocation.Contents.Invoke().ReadToEnd();
            }

            throw new liquid.Exceptions.FileSystemException("Template file {0} not found", new[] { templateName });
        }

        private string GetLocationQualifiedName(ViewLocationResult viewLocationResult)
        {
            return string.Concat(viewLocationResult.Location, "/", this.GetNeutralTemplateName(viewLocationResult.Name));
        }

        private string GetNeutralTemplateName(string templateName)
        {
            templateName = templateName
                .Replace(@"""", "")
                .Replace("'", "")
                .Replace(@"\", "/");

            templateName =
                this.extensionExpression.Replace(templateName, string.Empty);

            return templateName;
        }
    }
}