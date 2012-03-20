namespace Nancy.ViewEngines.DotLiquid
{
    using liquid = global::DotLiquid;    
    using System.IO;
    using System.Linq;

    public class LiquidNancyFileSystem : liquid.FileSystems.IFileSystem
    {
        private readonly ViewEngineStartupContext nancyContext;

        public LiquidNancyFileSystem(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.nancyContext = viewEngineStartupContext;
        }

        public string ReadTemplateFile(liquid.Context context, string templateName)
        {
            // Find the template referenced by the passed templateName
            ViewLocationResult viewLocation = nancyContext.ViewLocationResults.FirstOrDefault(v => ReplaceTroubleChars(v.Name) == ReplaceTroubleChars(templateName));

            if (viewLocation != null)
            {
                return viewLocation.Contents.Invoke().ReadToEnd();
            }

            throw new liquid.Exceptions.FileSystemException("Template file {0} not found", new[] { templateName });
        }

        private string ReplaceTroubleChars(string templateName)
        {
            return templateName.Replace(@"""", "").Replace("'", "").Replace("_","");
        }
    }
}