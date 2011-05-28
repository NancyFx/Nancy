namespace Nancy.ViewEngines.DotLiquid
{
    using liquid = global::DotLiquid;    

    public class LiquidNancyFileSystem : liquid.FileSystems.LocalFileSystem
    {
        public LiquidNancyFileSystem(string root) : base(root)
        {
        }

        public new virtual string ReadTemplateFile(liquid.Context context, string templateName)
        {
            return base.ReadTemplateFile(context, templateName);
        }
    }
}