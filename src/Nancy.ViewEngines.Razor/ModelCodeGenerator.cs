namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Globalization;
    using System.Web.Razor.Generator;

    public class ModelCodeGenerator : SetBaseTypeCodeGenerator
    {
        public ModelCodeGenerator(string baseType)
            : base(baseType)
        {
        }

        protected override string ResolveType(CodeGeneratorContext context, string baseType)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0}<{1}>",
                context.Host.DefaultBaseClass,
                baseType);
        }
    }
}